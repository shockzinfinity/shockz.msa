using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using RabbitMQ.Client;
using Serilog;
using shockz.msa.commonLogging;
using shockz.msa.eventBus.messages.Common;
using shockz.msa.ordering.api.Controllers;
using shockz.msa.ordering.api.EventBusConsumer;
using shockz.msa.ordering.api.Extensions;
using shockz.msa.ordering.application;
using shockz.msa.ordering.infrastructure;
using shockz.msa.ordering.infrastructure.Persistence;
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

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMassTransit(config =>
{
  config.AddHealthChecks();
  config.AddConsumer<BasketCheckoutConsumer>(); // ** IMPORTANT **
  config.UsingRabbitMq((ctx, cfg) =>
  {
    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
    {
      c.ConfigureConsumer<BasketCheckoutConsumer>(ctx); // ** IMPORTANT **
    });
  });
});

// General configuration
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<BasketCheckoutConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
  .AddDbContextCheck<OrderContext>()
  .AddRabbitMQ(_ =>
  {
    var factory = new ConnectionFactory
    {
      Uri = new Uri(builder.Configuration["EventBusSettings:HostAddress"]),
      AutomaticRecoveryEnabled = true
    };

    return factory.CreateConnection();
  });

builder.Services.AddOpenTelemetryTracing(traceBuiilder =>
{
  traceBuiilder.AddAspNetCoreInstrumentation()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddHttpClientInstrumentation()
    .AddSource(nameof(OrderController))
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
builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
  builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// migrate here
app.MigrateDatabase<OrderContext>((context, services) =>
{
  var logger = services.GetService<ILogger<OrderContextSeed>>();
  OrderContextSeed.SeedAsync(context, logger).Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering API v1");
    options.RoutePrefix = string.Empty;
  });
}

app.UseCors("corsapp");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions
{
  Predicate = _ => true,
  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
