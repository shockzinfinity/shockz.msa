using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;

namespace shockz.msa.identityServer.Data
{
  public class QuickStartContextSeed
  {
    public static void SeedAsync(ConfigurationDbContext context)
    {
      context.Database.Migrate();

      if (!context.Clients.Any()) {
        foreach (var client in Config.Clients) {
          context.Clients.Add(client.ToEntity());
        }
        context.SaveChanges();
      }

      if (!context.IdentityResources.Any()) {
        foreach (var resource in Config.IdentityResources) {
          context.IdentityResources.Add(resource.ToEntity());
        }
        context.SaveChanges();
      }

      if (!context.ApiScopes.Any()) {
        foreach (var scope in Config.ApiScopes) {
          context.ApiScopes.Add(scope.ToEntity());
        }
        context.SaveChanges();
      }
    }
  }
}
