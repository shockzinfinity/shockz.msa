using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Net.Http.Headers;
using shockz.msa.movie.client.HttpHandlers;
using shockz.msa.movie.client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMovieApiService, MovieApiService>();

builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services
  .AddHttpClient("MovieAPIClient", client =>
  {
    client.BaseAddress = new Uri("http://localhost:5256/");
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
  })
  .AddHttpMessageHandler<AuthenticationDelegatingHandler>();

builder.Services
  .AddHttpClient("IDPClient", client =>
  {
    client.BaseAddress = new Uri("https://localhost:7072/");
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
  });

builder.Services
  .AddSingleton(new ClientCredentialsTokenRequest
  {
    Address = "https://localhost:7072/connect/token",
    ClientId = "movieClient",
    ClientSecret = "secret",
    Scope = "movieAPI"
  });

builder.Services
  .AddAuthentication(options =>
  {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
  })
  .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
  .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
  {
    options.Authority = "https://localhost:7072";

    options.ClientId = "movies_mvc_client";
    options.ClientSecret = "secret";

    options.ResponseType = OidcConstants.ResponseTypes.Code;

    options.Scope.Add(OidcConstants.StandardScopes.OpenId);
    options.Scope.Add(OidcConstants.StandardScopes.Profile);

    options.SaveTokens = true;

    options.GetClaimsFromUserInfoEndpoint = true;
  });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
