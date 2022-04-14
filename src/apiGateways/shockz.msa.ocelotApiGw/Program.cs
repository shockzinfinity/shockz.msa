using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Serilog;
using shockz.msa.commonLogging;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog(SeriLogger.Configure);
// ocelot.Development.json or ocelot.Local.json
builder.Configuration.AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);
//builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
//builder.Logging.AddConsole();
//builder.Logging.AddDebug();

builder.Services.AddOcelot().AddCacheManager(settings => settings.WithDictionaryHandle());

var app = builder.Build();

app.MapGet("/", () => "Hello there.");
await app.UseOcelot(); // middleware

app.Run();
