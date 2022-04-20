using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using shockz.msa.commonLogging;
using shockz.msa.pollyPolicy;
using shockz.msa.shopping.Aggregator.Controllers;
using shockz.msa.shopping.Aggregator.Services;
using System.Diagnostics;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(loggingBuilder =>
{
  loggingBuilder.Configure(options =>
  {
    options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId;
  });
}).UseSerilog(SeriLogger.Configure);

builder.Services.AddTransient<LoggingDelegatingHandler>();

// Add services to the container.
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(PollyPolicy.GetRetryPolicy())
  .AddPolicyHandler(PollyPolicy.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(PollyPolicy.GetRetryPolicy())
  .AddPolicyHandler(PollyPolicy.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderingService, OrderingService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(PollyPolicy.GetRetryPolicy())
  .AddPolicyHandler(PollyPolicy.GetCircuitBreakerPolicy());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
  .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:CatalogUrl"]}/index.html"), "Catalog.API", HealthStatus.Degraded)
  .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:BasketUrl"]}/index.html"), "Basket.API", HealthStatus.Degraded)
  .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:OrderingUrl"]}/index.html"), "Ordering.API", HealthStatus.Degraded);

builder.Services.AddOpenTelemetryTracing(traceBuiilder =>
{
  traceBuiilder.AddAspNetCoreInstrumentation()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddHttpClientInstrumentation()
    .AddSource(nameof(ShoppingController))
    .AddJaegerExporter(options =>
    {
      options.AgentHost = builder.Configuration.GetValue<string>("OpenTelmetry:Host");
      options.AgentPort = builder.Configuration.GetValue<int>("OpenTelmetry:Port");
      options.ExportProcessorType = ExportProcessorType.Simple;
    })
    .AddConsoleExporter(options =>
    {
      options.Targets = ConsoleExporterOutputTargets.Console;
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Shopping Aggregator API v1");
    options.RoutePrefix = string.Empty;
  });
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions
{
  Predicate = _ => true,
  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
