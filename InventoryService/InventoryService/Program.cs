using BLL.Interfaces;
using BLL.Services;
using DAL.Data;
using DAL.Interfaces;
using DAL.Repositories;
using InventoryService.Extensions;
using InventoryService.Grpc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;

// Enable HTTP/2 unencrypted support for gRPC
AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddGatewayHeaderAuth(builder.Configuration);

// Configure DbContext
builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure Repositories
builder.Services.AddScoped<IDealerRepository, DealerRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

// Configure Services
builder.Services.AddScoped<IInventoryService, BLL.Services.InventoryService>();
builder.Services.AddScoped<IDealerService, DealerService>();

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

// CORS
app.UseCors("AllowAllOrigins");

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// gRPC
app.MapGrpcService<InventoryGrpcService>();

// Liveness endpoint for Docker/K8s
app.MapHealthChecks("/health");

app.Run();
