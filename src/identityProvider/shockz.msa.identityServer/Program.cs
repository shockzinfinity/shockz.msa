using shockz.msa.identityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddIdentityServer()
  .AddInMemoryClients(Config.Clients)
  .AddInMemoryApiScopes(Config.ApiScopes)
  .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer(); // TODO: more configures

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
