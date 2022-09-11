using IdentityModel;
using Microsoft.AspNetCore.Authentication;
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
    //client.BaseAddress = new Uri("http://localhost:5256/"); // direct to api
    client.BaseAddress = new Uri("http://localhost:5300/"); // via api gateway
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

//builder.Services
//  .AddSingleton(new ClientCredentialsTokenRequest
//  {
//    Address = "https://localhost:7072/connect/token",
//    ClientId = "movieClient",
//    ClientSecret = "secret",
//    Scope = "movieAPI"
//  });

builder.Services.AddHttpContextAccessor();

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

    //options.ResponseType = OidcConstants.ResponseTypes.Code;
    options.ResponseType = OidcConstants.ResponseTypes.CodeIdToken; // hybrid

    //options.Scope.Add(OidcConstants.StandardScopes.OpenId); // auto included
    //options.Scope.Add(OidcConstants.StandardScopes.Profile); // auto included
    options.Scope.Add("movieAPI");
    options.Scope.Add(OidcConstants.StandardScopes.Address);
    options.Scope.Add(OidcConstants.StandardScopes.Email);
    options.Scope.Add("roles");

    options.ClaimActions.MapUniqueJsonKey("role", "role");

    options.SaveTokens = true;

    options.GetClaimsFromUserInfoEndpoint = true;

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
      NameClaimType = JwtClaimTypes.GivenName,
      RoleClaimType = JwtClaimTypes.Role
    };
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
