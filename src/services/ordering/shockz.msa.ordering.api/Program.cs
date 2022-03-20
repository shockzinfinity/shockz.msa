using MassTransit;
using shockz.msa.eventBus.messages.Common;
using shockz.msa.ordering.api.EventBusConsumer;
using shockz.msa.ordering.api.Extensions;
using shockz.msa.ordering.application;
using shockz.msa.ordering.infrastructure;
using shockz.msa.ordering.infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddMassTransit(config =>
{
  config.AddConsumer<BasketCheckoutConsumer>(); // ** IMPORTANT **
  config.UsingRabbitMq((ctx, cfg) =>
  {
    cfg.Host(builder.Configuration["EventBusSettings:HostAddress"]);
    cfg.ReceiveEndpoint(EventBusConstants.BasketCheckoutQueue, c =>
    {
      c.ConfigureConsumer<BasketCheckoutConsumer>(ctx); // ** IMPORTANT **
    });
  });
});

// General configuration
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<BasketCheckoutConsumer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// migrate here
app.MigrateDatabase<OrderContext>((context, services) =>
{
  var logger = services.GetService<ILogger<OrderContextSeed>>();
  OrderContextSeed.SeedAsync(context, logger).Wait();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering API v1");
    options.RoutePrefix = string.Empty;
  });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
