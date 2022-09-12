using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using shockz.msa.movie.client.Models;

namespace shockz.msa.movie.client.Services
{
  public class UserService : IUserService
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public UserService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
    {
      _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
      _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
    }

    public async Task<UserInfoViewModel> GetUserInfo()
    {
      var idpClient = _httpClientFactory.CreateClient(shockz.msa.common.Constant.Http_Client_Idp);
      var metaDataResponse = await idpClient.GetDiscoveryDocumentAsync();
      if (metaDataResponse.IsError) {
        throw new HttpRequestException("Something went wrong while requesting the access token");
      }

      var accessToken = await _contextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);
      var userInfoResponse = await idpClient.GetUserInfoAsync(
        new UserInfoRequest
        {
          Address = metaDataResponse.UserInfoEndpoint,
          Token = accessToken
        });

      if (userInfoResponse.IsError) {
        throw new HttpRequestException("Something went wrong while getting user info");
      }

      var userinfoDictionary = new Dictionary<string, string>();
      foreach (var claim in userInfoResponse.Claims) {
        userinfoDictionary.Add(claim.Type, claim.Value);
      }

      return new UserInfoViewModel(userinfoDictionary);
    }
  }
}
