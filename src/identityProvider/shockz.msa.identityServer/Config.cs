using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

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
        ClientId = "movies_mvc_client",
        ClientName = "Movies MVC Web App",
        AllowedGrantTypes = GrantTypes.Hybrid,
        RequirePkce = false,
        AllowRememberConsent = false,
        RedirectUris = new List<string>()
        {
          "https://localhost:7216/signin-oidc" // this is client app port
        },
        PostLogoutRedirectUris = new List<string>()
        {
          "https://localhost:7216/signout-callback-oidc"
        },
        ClientSecrets = new List<Secret>
        {
          new Secret("secret".Sha256())
        },
        AllowedScopes = new List<string>()
        {
          IdentityServerConstants.StandardScopes.OpenId,
          IdentityServerConstants.StandardScopes.Profile,
          "movieAPI"
        }
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
      new IdentityResources.OpenId(),
      new IdentityResources.Profile()
    };

    public static List<TestUser> TestUsers => new List<TestUser>
    {
      new TestUser
      {
        SubjectId = "6012C74D-6FF2-44D1-90F3-60DBB75831D9",
        Username = "shockz",
        Password = "shockz",
        Claims = new List<Claim>
        {
          new Claim(JwtClaimTypes.GivenName, "Jun"),
          new Claim(JwtClaimTypes.FamilyName, "Yu")
        }
      }
    };
  }
}
