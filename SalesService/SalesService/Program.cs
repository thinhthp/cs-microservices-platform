using Microsoft.EntityFrameworkCore;
using SalesService.BLL.Grpcs.Inventory;
using SalesService.BLL.Grpcs.Product;
using SalesService.BLL.Services.Customers;
using SalesService.BLL.Services.Order;
using SalesService.BLL.Services.Payment;
using SalesService.DAL.Common;
using SalesService.DAL.Data;
using SalesService.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddControllers();
builder.Services.AddGatewayHeaderAuth(builder.Configuration);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

// Add db
builder.Services.AddDbContext<SalesContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Grpc
// Product service
builder.Services.AddGrpcClient<ProductService.Grpc.ProductGrpc.ProductGrpcClient>(o =>
{
    o.Address = new Uri("http://productservice:80");
});
builder.Services.AddScoped<IProductGrpcClient, ProductGrpcClient>();

// Inventory service
builder.Services.AddGrpcClient<InventoryService.Grpc.InventoryGrpc.InventoryGrpcClient>(o =>
{
    o.Address = new Uri("http://inventoryservice:80");
});
builder.Services.AddScoped<IInventoryGrpcClient, InventoryGrpcClient>();

var app = builder.Build();

// Apply pending migrations at startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<SalesContext>();
        await db.Database.MigrateAsync();
        logger.LogInformation("Database migrated successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

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

// Liveness endpoint for Docker/K8s
app.MapHealthChecks("/health");

app.Run();
