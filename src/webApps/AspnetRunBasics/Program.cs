using AspnetRunBasics.Data;
using AspnetRunBasics.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AspnetRunContext>(c => c.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();

builder.Services.AddRazorPages();

var app = builder.Build();

SeedDatabase(app);

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

static void SeedDatabase(IHost host)
{
  using (var scope = host.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();

    try {
      var context = services.GetRequiredService<AspnetRunContext>();
      AspnetRunContextSeed.SeedAsync(context, loggerFactory).Wait();
    } catch (Exception ex) {
      var logger = loggerFactory.CreateLogger<Program>();
      logger.LogError(ex, "An error occurred seeding the DB.");
      throw;
    }
  }
}
