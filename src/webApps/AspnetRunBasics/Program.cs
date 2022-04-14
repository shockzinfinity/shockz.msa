using AspnetRunBasics.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly.Extensions.Http;
using Polly;
using Serilog;
using shockz.msa.commonLogging;
using System;
using System.Net.Http;

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

//builder.Services.AddDbContext<AspnetRunContext>(c => c.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddScoped<IProductRepository, ProductRepository>();
//builder.Services.AddScoped<ICartRepository, CartRepository>();
//builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//builder.Services.AddScoped<IContactRepository, ContactRepository>();

builder.Services.AddTransient<LoggingDelegatingHandler>();

builder.Services.AddHttpClient<ICatalogService, CatalogService>(h =>
  h.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(GetRetryPolicy())
  .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IBasketService, BasketService>(h =>
  h.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(GetRetryPolicy())
  .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient<IOrderService, OrderService>(h =>
  h.BaseAddress = new Uri(builder.Configuration["ApiSettings:GatewayAddress"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>()
  .AddPolicyHandler(GetRetryPolicy())
  .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddRazorPages();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
  app.UseDeveloperExceptionPage();
} else {
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
  endpoints.MapRazorPages();
});

//app.Run();
await app.RunAsync();
