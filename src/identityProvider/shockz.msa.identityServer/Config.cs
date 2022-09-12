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
      //new Client
      //{
      //  ClientId = "movieClient",
      //  AllowedGrantTypes = GrantTypes.ClientCredentials,
      //  ClientSecrets = { new Secret("secret".Sha256()) },
      //  AllowedScopes = { "movieAPI" }
      //},
      new Client
      {
        ClientId = shockz.msa.common.Constant.Movies_Client_Id_Value,
        ClientName = shockz.msa.common.Constant.Movies_Client_Name,
        AllowedGrantTypes = GrantTypes.Hybrid,
        RequirePkce = false,
        AllowRememberConsent = false,
        RedirectUris = new List<string>() { shockz.msa.common.Url.Sign_In },
        PostLogoutRedirectUris = new List<string>() { shockz.msa.common.Url.Sign_Out },
        ClientSecrets = new List<Secret>
        {
          new Secret(shockz.msa.common.Constant.Movies_Client_Secret.Sha256())
        },
        AllowedScopes = new List<string>()
        {
          IdentityServerConstants.StandardScopes.OpenId,
          IdentityServerConstants.StandardScopes.Profile,
          IdentityServerConstants.StandardScopes.Address,
          IdentityServerConstants.StandardScopes.Email,
          shockz.msa.common.Constant.Scope_Role_Value,
          shockz.msa.common.Constant.Scope_Movie_Api_Value
        }
      }
    };

    public static IEnumerable<ApiScope> ApiScopes => new ApiScope[]
    {
      new ApiScope(shockz.msa.common.Constant.Scope_Movie_Api_Value, shockz.msa.common.Constant.Scope_Movie_Api_Text)
    };

    public static IEnumerable<ApiResource> ApiResources => new ApiResource[]
    {
    };

    public static IEnumerable<IdentityResource> IdentityResources => new IdentityResource[]
    {
      new IdentityResources.OpenId(),
      new IdentityResources.Profile(),
      new IdentityResources.Address(),
      new IdentityResources.Email(),
      new IdentityResource(shockz.msa.common.Constant.Scope_Role_Value, shockz.msa.common.Constant.Scope_Role_Text, new List<string>{ shockz.msa.common.Constant.Scope_Role_Value })
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
