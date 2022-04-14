using Serilog;
using shockz.msa.commonLogging;
using shockz.msa.shopping.Aggregator.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

builder.Services.AddTransient<LoggingDelegatingHandler>();

// Add services to the container.
builder.Services.AddHttpClient<ICatalogService, CatalogService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:CatalogUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>();
builder.Services.AddHttpClient<IBasketService, BasketService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:BasketUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>();
builder.Services.AddHttpClient<IOrderingService, OrderingService>(c =>
  c.BaseAddress = new Uri(builder.Configuration["ApiSettings:OrderingUrl"]))
  .AddHttpMessageHandler<LoggingDelegatingHandler>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.Run();
