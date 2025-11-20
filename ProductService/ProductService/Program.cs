using BLL.Interfaces;
using BLL.Services;
using DAL.Data.DbFirst;
using DAL.Interfaces;
using DAL.Repositories;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using ProductService.Extensions;
using ProductService.Grpc;

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
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );

// gRPC server
builder.Services.AddGrpc();
// Kestrel: allow HTTP/1.1 + HTTP/2 on port 80 (h2c for gRPC)
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80, o => o.Protocols = HttpProtocols.Http1AndHttp2);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// gRPC
app.MapGrpcService<ProductGrpcService>();

// Liveness endpoint for Docker/K8s
app.MapHealthChecks("/health");

app.UseCors("AllowAllOrigins");

app.Run();
