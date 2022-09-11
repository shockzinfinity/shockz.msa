using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using shockz.msa.movie.client.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMovieApiService, MovieApiService>();

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

    options.ResponseType = "code";

    options.Scope.Add("openid");
    options.Scope.Add("profile");

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
