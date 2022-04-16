using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using shockz.msa.commonLogging;
using shockz.msa.shopping.Aggregator.Services;

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
  // 2 ^ 1 = 2s
  // 2 ^ 2 = 4s
  // 2 ^ 3 = 8s
  // 2 ^ 4 = 16s
  // 2 ^ 5 = 32s

  return HttpPolicyExtensions
    .HandleTransientHttpError()
    .WaitAndRetryAsync(
      retryCount: 5,
      sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
      onRetry: (exception, retryCount, context) =>
      {
        Log.Error($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}");
      });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
  return HttpPolicyExtensions
    .HandleTransientHttpError()
    .CircuitBreakerAsync(
      handledEventsAllowedBeforeBreaking: 5,
      durationOfBreak: TimeSpan.FromSeconds(30));
}

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddTransient<LoggingDelegatingHandler>();

// Add services to the container.
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(GetRetryPolicy())
  .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(GetRetryPolicy())
  .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderingService, OrderingService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(GetRetryPolicy())
  .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
  .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:CatalogUrl"]}/index.html"), "Catalog.API", HealthStatus.Degraded)
  .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:BasketUrl"]}/index.html"), "Basket.API", HealthStatus.Degraded)
  .AddUrlGroup(new Uri($"{builder.Configuration["ApiSettings:OrderingUrl"]}/index.html"), "Ordering.API", HealthStatus.Degraded);

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
