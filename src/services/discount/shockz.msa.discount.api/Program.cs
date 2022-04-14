using Serilog;
using shockz.msa.commonLogging;
using shockz.msa.discount.api.Extensions;
using shockz.msa.discount.api.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);

// Add services to the container.
builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
//Debug.WriteLine(builder.Configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MigrateDatabase<Program>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Discount API v1");
    options.RoutePrefix = string.Empty;
  });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
