﻿using IdentityServer4.Models;
using IdentityServer4.Test;

namespace shockz.msa.identityServer
{
  public class Config
  {
    public static IEnumerable<Client> Clients => new Client[]
    {
      new Client
      {
        ClientId = "movieClient",
        AllowedGrantTypes = GrantTypes.ClientCredentials,
        ClientSecrets = { new Secret("secret".Sha256()) },
        AllowedScopes = { "movieAPI" }
      },
      new Client
      {
        ClientId = "movieClient2",
        AllowedGrantTypes = GrantTypes.ClientCredentials,
        ClientSecrets = { new Secret("secret".Sha256()) },
        AllowedScopes = { "movieAPI" }
      }
    };

    public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
    {
      new ApiScope("movieAPI", "Movie API")
    };

    public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
    {
    };

    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
    };

    public static List<TestUser> TestUsers => new List<TestUser>
    {
    };
  }
}
