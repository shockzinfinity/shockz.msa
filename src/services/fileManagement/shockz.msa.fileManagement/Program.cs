using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using shockz.msa.commonLogging;
using shockz.msa.fileManagement.Endpoints;
using shockz.msa.fileManagement.Services;
using System.Text;
using tusdotnet;
using tusdotnet.Helpers;
using tusdotnet.Interfaces;
using tusdotnet.Models;
using tusdotnet.Models.Expiration;
using tusdotnet.Stores;

var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(loggingBuilder =>
{
  loggingBuilder.Configure(options =>
  {
    options.ActivityTrackingOptions = ActivityTrackingOptions.TraceId | ActivityTrackingOptions.SpanId;
  });
}).UseSerilog(SeriLogger.Configure);
builder.WebHost.ConfigureKestrel((context, options) =>
{
  options.Limits.MaxRequestBodySize = 10L * 1024L * 1024L * 1024L; // 10 GB
});

// Add services to the container.
builder.Services.AddSingleton<DefaultTusConfiguration>(s => CreateTusConfiguration(s, builder.Environment));
builder.Services.AddHostedService<ExpiredFilesCleanupService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOpenTelemetryTracing(traceBuiilder =>
{
  traceBuiilder.AddAspNetCoreInstrumentation()
    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(builder.Environment.ApplicationName))
    .AddHttpClientInstrumentation()
    //.AddSource(nameof(DiscountController))
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

builder.Services.AddCors();
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().WithExposedHeaders(CorsHelper.GetExposedHeaders()));

app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions
{
  Predicate = _ => true,
  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.UseTus(httpContext => httpContext.RequestServices.GetRequiredService<DefaultTusConfiguration>());
app.MapGet("/files/{fileId}", DownloadFileEndpoint.HandleRoute);
app.Run();

static DefaultTusConfiguration CreateTusConfiguration(IServiceProvider serviceProvider, IWebHostEnvironment env)
{
  var tusFiles = Path.Combine(env.ContentRootPath, "tusfiles");

  var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

  return new DefaultTusConfiguration
  {
    Store = new TusDiskStore(tusFiles),
    UrlPath = "/files",
    MetadataParsingStrategy = MetadataParsingStrategy.AllowEmptyValues,
    UsePipelinesIfAvailable = true,
    Expiration = new AbsoluteExpiration(TimeSpan.FromMinutes(5)),
    Events = new tusdotnet.Models.Configuration.Events
    {
      OnFileCompleteAsync = async eventContext =>
      {
        // eventContext.FileId is the id of the file that was uploaded.
        // eventContext.Store is the data store that was used (in this case an instance of the TusDiskStore)

        // A normal use case here would be to read the file and do some processing on it.
        //ITusFile file = await eventContext.GetFileAsync();
        //var metadata = await file.GetMetadataAsync(eventContext.CancellationToken);
        //var fileNameMetadata = metadata["name"];
        //var fileName = fileNameMetadata.GetString(Encoding.UTF8);
        //var extensionsName = Path.GetExtension(fileName);

        // TODO: move to another disk store (or external storage) - e.g. AWS S3 with Hangfire thread
        //File.Move(Path.Combine(tusFiles, eventContext.FileId), Path.Combine(tusFiles, $"{eventContext.FileId}{extensionsName}"));

        logger.LogInformation($"Upload of {eventContext.FileId} completed using {eventContext.Store.GetType().FullName}");
      }
    }
  };

}
