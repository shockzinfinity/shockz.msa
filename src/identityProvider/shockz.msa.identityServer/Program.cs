using shockz.msa.identityServer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentityServer()
  .AddInMemoryClients(Config.Clients)
  .AddInMemoryApiScopes(Config.ApiScopes)
  .AddDeveloperSigningCredential();

var app = builder.Build();

app.UseRouting();
app.UseIdentityServer(); // TODO: more configures

//app.MapGet("/", () => "Hello World!");
app.UseEndpoints(endpoints =>
{
  endpoints.MapGet("/", async context =>
  {
    await context.Response.WriteAsync("Hello there...");
  });
});

app.Run();
