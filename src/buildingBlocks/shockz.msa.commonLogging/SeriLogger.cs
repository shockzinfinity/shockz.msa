using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Diagnostics;

namespace shockz.msa.commonLogging;

public static class SeriLogger
{
  public static Action<HostBuilderContext, LoggerConfiguration> Configure => (context, loggerConfiguration) =>
   {
     var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
     var elasticUser = context.Configuration.GetValue<string>("ElasticConfiguration:Username");
     var elasticPassword = context.Configuration.GetValue<string>("ElasticConfiguration:Password");

     loggerConfiguration
       .Enrich.FromLogContext()
       .Enrich.WithExceptionDetails()
       .Enrich.WithMachineName()
       .Enrich.WithSpan()
       .Enrich.WithCorrelationId()
       .WriteTo.Debug()
       .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {TraceId} {Level:u3} {Message}{NewLine}{Exception}")
       .WriteTo.Elasticsearch(
         new ElasticsearchSinkOptions(new Uri(elasticUri))
         {
           IndexFormat = $"ecommerce-{context.HostingEnvironment.ApplicationName?.ToLower().Replace(".", "-")}-{context.HostingEnvironment.EnvironmentName?.ToLower().Replace(".", "-")}-logs-{DateTime.UtcNow:yyyy-MM}",
           AutoRegisterTemplate = true,
           AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
           TypeName = null,
           FailureCallback = x => Console.WriteLine(x),
           ModifyConnectionSettings = x =>
             x.ConnectionLimit(-1)
              .BasicAuthentication(elasticUser, elasticPassword)
              .ServerCertificateValidationCallback((source, certificate, chain, sslPolicyErrors) => { return true; }),
           NumberOfShards = 2,
           NumberOfReplicas = 1,
           BatchAction = ElasticOpType.Create
         })
       .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
       .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName)
       .Enrich.With<LogEnricher>()
       .ReadFrom.Configuration(context.Configuration);

     Serilog.Debugging.SelfLog.Enable(message => Console.WriteLine(message));
   };
}
