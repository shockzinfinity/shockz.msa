using AspnetRunBasics.Services;
using AspnetRunBasics.Settings;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using shockz.msa.commonLogging;
using shockz.msa.pollyPolicy;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(loggingBuilder =>
{
  loggingBuilder.Configure(options =>
  {
    options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId;
  });
}).UseSerilog(SeriLogger.Configure);

builder.Services.AddTransient<LoggingDelegatingHandler>();
builder.Services.Configure<ApiSettings>(c => builder.Configuration.GetSection("ApiSettings")); // in case needed by other services

builder.Services.AddHttpClient<ICatalogService, CatalogService>(h =>
  h.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(PollyPolicy.GetRetryPolicy())
  .AddPolicyHandler(PollyPolicy.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(h =>
  h.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(PollyPolicy.GetRetryPolicy())
  .AddPolicyHandler(PollyPolicy.GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderService, OrderService>(h =>
  h.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(PollyPolicy.GetRetryPolicy())
  .AddPolicyHandler(PollyPolicy.GetCircuitBreakerPolicy());

builder.Services.AddRazorPages();
builder.Services.AddHealthChecks()
  .AddUrlGroup(new Uri(builder.Configuration["ApiSettings:GatewayAddress"]), "Ocelot API Gateway", HealthStatus.Degraded);

builder.Services.AddOpenTelemetryTracing(traceBuiilder =>
{
  traceBuiilder.AddAspNetCoreInstrumentation()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddHttpClientInstrumentation()
    .AddSource(nameof(CatalogService))
    .AddSource(nameof(BasketService))
    .AddSource(nameof(OrderService))
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

if (app.Environment.IsDevelopment()) {
  app.UseDeveloperExceptionPage();
} else {
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
  endpoints.MapRazorPages();
  endpoints.MapHealthChecks("/hc", new HealthCheckOptions
  {
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
  });
});

//app.Run();
await app.RunAsync();
