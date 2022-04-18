using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace shockz.msa.commonLogging;

public static class SeriLogger
{
  public static Action<HostBuilderContext, LoggerConfiguration> Configure => (context, loggerConfiguration) =>
   {
     var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");

     loggerConfiguration
       .Enrich.FromLogContext()
       .Enrich.WithMachineName()
       .WriteTo.Debug()
       .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {TraceId} {Level:u3} {Message}{NewLine}{Exception}")
       .WriteTo.Elasticsearch(
         new ElasticsearchSinkOptions(new Uri(elasticUri))
         {
           IndexFormat = $"applogs-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
           AutoRegisterTemplate = true,
           NumberOfShards = 2,
           NumberOfReplicas = 1
         })
       .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
       .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
       .ReadFrom.Configuration(context.Configuration);
   };
}
