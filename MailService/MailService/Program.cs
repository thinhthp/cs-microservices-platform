using MailService.Extensions;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRabbitMqEmailConsumer(builder.Configuration);
builder.Services.AddSendGridEmail(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

// Metrics
//builder.Services.AddOpenTelemetry()
//    .ConfigureResource(r => r.AddService(builder.Environment.ApplicationName))
//    .WithMetrics(meterProviderBuilder =>
//    {
//        meterProviderBuilder
//            .AddAspNetCoreInstrumentation()
//            .AddHttpClientInstrumentation()
//            .AddRuntimeInstrumentation()
//            .AddProcessInstrumentation()
//            .AddPrometheusExporter();
//    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Liveness endpoint for Docker/K8s
app.MapHealthChecks("/health");
app.MapGet("/", () => "API is running!");

// Prometheus metrics endpoint
//app.MapPrometheusScrapingEndpoint();

app.Run();
