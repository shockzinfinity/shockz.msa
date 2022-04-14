using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;
using shockz.msa.catalog.api.Data;
using shockz.msa.catalog.api.Repositories;
using shockz.msa.commonLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.
builder.Services.AddScoped<ICatalogContext, CatalogContext>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IPriceRepository, PriceRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
  .AddMongoDb(builder.Configuration["DatabaseSettings:ConnectionString"], "Catalog MongoDb Health", HealthStatus.Degraded);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Catalog API v1");
    options.RoutePrefix = string.Empty;
  });
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/hc", new HealthCheckOptions()
{
  Predicate = _ => true,
  ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();
