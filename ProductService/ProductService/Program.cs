using BLL.Interfaces;
using BLL.Services;
using DAL.Data.DbFirst;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using ProductService.Extensions;
using ProductService.Grpc;

// Enable HTTP/2 unencrypted support for gRPC
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddGatewayHeaderAuth(builder.Configuration);

// Configure DbContext
builder.Services.AddDbContext<ProductDbFirstContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure Repositories
builder.Services.AddScoped<IModelRepository, ModelRepository>();
builder.Services.AddScoped<IVariantRepository, VariantRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();

// Configure Services
builder.Services.AddScoped<IVariantService, VariantService>();
builder.Services.AddScoped<IModelService, ModelService>();
builder.Services.AddScoped<IBrandService, BrandService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

// add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAllOrigins",
        policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );
});

// gRPC server
builder.Services.AddGrpc();

// Kestrel: allow HTTP/1.1 + HTTP/2 on port 80 (h2c for gRPC)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80, o => o.Protocols = HttpProtocols.Http1);          // REST
    options.ListenAnyIP(5001, o => o.Protocols = HttpProtocols.Http2);        // gRPC h2c (no TLS)
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowAllOrigins");

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// gRPC
app.MapGrpcService<ProductGrpcService>();

// Liveness endpoint for Docker/K8s
app.MapHealthChecks("/health");

app.Run();
