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

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddHealthChecksUI().AddInMemoryStorage();

builder.Services.AddOpenTelemetryTracing(traceBuiilder =>
{
  traceBuiilder.AddAspNetCoreInstrumentation()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddHttpClientInstrumentation()
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
if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapHealthChecksUI();
app.MapRazorPages();

app.Run();
