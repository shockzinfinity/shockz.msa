using IdentityModel.Client;

namespace shockz.msa.movie.client.HttpHandlers
{
  public class AuthenticationDelegatingHandler : DelegatingHandler
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ClientCredentialsTokenRequest _tokenRequest;

    public AuthenticationDelegatingHandler(IHttpClientFactory httpClientFactory, ClientCredentialsTokenRequest tokenRequest)
    {
      _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
      _tokenRequest = tokenRequest ?? throw new ArgumentNullException(nameof(tokenRequest));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var httpClient = _httpClientFactory.CreateClient("IDPClient");

      var tokenResponse = await httpClient.RequestClientCredentialsTokenAsync(_tokenRequest);
      if (tokenResponse.IsError) {
        throw new HttpRequestException("Something went wrong while requesting the access token");
      }

      request.SetBearerToken(tokenResponse.AccessToken);

      return await base.SendAsync(request, cancellationToken);
    }
  }
}
