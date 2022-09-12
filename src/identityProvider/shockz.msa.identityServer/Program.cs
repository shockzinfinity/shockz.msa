using IdentityServer4.EntityFramework.DbContexts;
using IdentityServerHost.Quickstart.UI;
using Microsoft.EntityFrameworkCore;
using shockz.msa.identityServer.Data;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

var migrationAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;
var connectionString = builder.Configuration.GetConnectionString(shockz.msa.common.Constant.Connection_String_Identity_Server_Key);

//builder.Services.AddIdentityServer()
//  .AddInMemoryClients(Config.Clients)
//  .AddInMemoryApiScopes(Config.ApiScopes)
//  .AddInMemoryIdentityResources(Config.IdentityResources)
//  .AddTestUsers(TestUsers.Users)
//  .AddDeveloperSigningCredential();
builder.Services.AddIdentityServer()
  .AddTestUsers(TestUsers.Users)
  .AddConfigurationStore(options =>
  {
    options.ConfigureDbContext = b => b.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
  })
  .AddOperationalStore(options =>
  {
    options.ConfigureDbContext = b => b.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationAssembly));
  })
  .AddDeveloperSigningCredential();

var app = builder.Build();

using var serviceScope = app.Services.CreateScope();
serviceScope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
QuickStartContextSeed.SeedAsync(context);

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer(); // TODO: more configures

app.UseAuthorization();

//app.MapGet("/", () => "Hello World!");
//app.UseEndpoints(endpoints =>
//{
//  endpoints.MapGet("/", async context =>
//  {
//    await context.Response.WriteAsync("Hello there...");
//  });
//});
app.UseEndpoints(endpoints =>
{
  endpoints.MapDefaultControllerRoute();
});

app.Run();
