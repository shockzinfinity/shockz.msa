using MassTransit;
using shockz.msa.basket.api.GrpcServices;
using shockz.msa.basket.api.Repositories;
using shockz.msa.discount.grpc.Protos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Redis configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
  options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});

// General configuration
builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddAutoMapper(typeof(Program));

// Grpc configuration
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
  options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]);
});
builder.Services.AddScoped<DiscountGrpcService>();

// MassTransit, RabbitMQ configuration
builder.Services.AddMassTransit(config =>
{
  config.UsingRabbitMq((ctx, cfg) =>
  {
    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
  });
});
//builder.Services.AddMassTransitHostedService(); // NOTE: obsolete

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
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Basket API v1");
    options.RoutePrefix = string.Empty;
  });
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
