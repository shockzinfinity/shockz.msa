using Microsoft.EntityFrameworkCore;
using shockz.msa.movie.api.Data;
using shockz.msa.movie.api.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MoviesAPIContext>(options => options.UseInMemoryDatabase("Movies"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(shockz.msa.common.Constant.Authentication_Scheme_Bearer)
  .AddJwtBearer(shockz.msa.common.Constant.Authentication_Scheme_Bearer, options =>
  {
    options.Authority = shockz.msa.common.Url.Identity_Server;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
      ValidateAudience = false
    };
  });

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy(shockz.msa.common.Constant.Client_Id_Policy, policy =>
    policy.RequireClaim(shockz.msa.common.Constant.Movies_Client_Id_Key, shockz.msa.common.Constant.Movies_Client_Id_Value));
});

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
  builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
var moviesAPIContext = services.GetRequiredService<MoviesAPIContext>();
MoviesContextSeed.SeedAsync(moviesAPIContext);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseCors("corsapp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
