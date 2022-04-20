using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using shockz.msa.commonLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(loggingBuilder =>
{
  loggingBuilder.Configure(options =>
  {
    options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId;
  });
}).UseSerilog(SeriLogger.Configure);
// ocelot.Development.json or ocelot.Local.json
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);
//builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();

builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

builder.Services.AddOpenTelemetryTracing(traceBuiilder =>
{
  traceBuiilder.AddAspNetCoreInstrumentation()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddHttpClientInstrumentation()
    .AddSource(nameof(IOcelotBuilder))
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

app.UseRouting();
app.UseEndpoints(endpoints =>
{
  endpoints.MapGet("/", async context =>
  {
    await context.Response.WriteAsync("Hello there.");
  });
});

await app.UseOcelot(); // middleware

app.Run();
