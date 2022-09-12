using shockz.msa.movie.client.Extensions;
using shockz.msa.movie.client.Models;

namespace shockz.msa.movie.client.Services
{
  public class MovieApiService : IMovieApiService
  {
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _contextAccessor;

    public MovieApiService(IHttpClientFactory httpClientFactory, IHttpContextAccessor contextAccessor)
    {
      _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
      _contextAccessor = contextAccessor ?? throw new ArgumentNullException(nameof(contextAccessor));
    }

    public async Task<IEnumerable<Movie>> GetMovies()
    {
      #region test codes

      //// 1 - Get token from identity server, of course we should provide the IS configuration like address, clientid and clientsecret.
      //// 2 - Send request to Protected API
      //// 3 - deserialize object to movie list

      //// 1. "retrive" our api credentials. This must be registered on Identity Server
      //var apiClientCredentials = new ClientCredentialsTokenRequest
      //{
      //  Address = "https://localhost:7072/connect/token",
      //  ClientId = "movieClient",
      //  ClientSecret = "secret",

      //  // This is the scope our Protected API requires.
      //  Scope = "movieAPI"
      //};

      //// create a new httpclient to talk to our identity server
      //var client = new HttpClient();

      //// just check if we can reach the Discovery document. Not 100% needed but ..
      //var disco = await client.GetDiscoveryDocumentAsync("https://localhost:7072");
      //if (disco.IsError) {
      //  return null; // throw 500 error
      //}

      //// 2. Authenticates and get an access token from Identity Server
      //var tokenResponse = await client.RequestClientCredentialsTokenAsync(apiClientCredentials);
      //if (tokenResponse.IsError) {
      //  return null;
      //}

      //// 2. Send request to PRotected API
      //// another HttpClient for talking now with our Protected API
      //var apiClient = new HttpClient();

      //// 3. Set the access_token in the request Authorization: Bearer <token>
      //apiClient.SetBearerToken(tokenResponse.AccessToken);

      ////var response = await apiClient.GetAsync("http://localhost:5256/api/movies");

      ////response.EnsureSuccessStatusCode();

      ////var moviesList = await JsonSerializer.DeserializeAsync<List<Movie>>(await response.Content.ReadAsStreamAsync(), new JsonSerializerOptions { PropertyNameCaseInsensitive = false });
      //var moviesList = await apiClient.GetFromJsonAsync<List<Movie>>("http://localhost:5256/api/movies");

      //return moviesList;

      #endregion test codes

      var movies = await Get<List<Movie>>(shockz.msa.common.Url.Movies);

      return movies;
    }

    public async Task<Movie> GetMovie(int id)
    {
      var movie = await Get<Movie>(string.Format(shockz.msa.common.Url.Movies_Id, id));
      return movie;
    }

    public async Task<bool> CreateMovie(Movie movie)
    {
      await Execute<Movie>(HttpMethod.Post, shockz.msa.common.Url.Movies, movie);

      return true;
    }

    public async Task<bool> UpdateMovie(int id, Movie movie)
    {
      await Execute<Movie>(HttpMethod.Put, string.Format(shockz.msa.common.Url.Movies_Id, id), movie);

      return true;
    }

    public async Task<bool> DeleteMovice(int id)
    {
      await Execute<Movie>(HttpMethod.Delete, string.Format(shockz.msa.common.Url.Movies_Id, id), default);

      return true;
    }

    private async Task<T> Get<T>(string url)
    {
      return await Execute<T>(HttpMethod.Get, url, default);
    }

    private async Task<T> Execute<T>(HttpMethod method, string uri, T data)
    {
      var httpClient = _httpClientFactory.CreateClient(shockz.msa.common.Constant.Http_Client_Movies_Api);
      var request = new HttpRequestMessage(method, uri);
      if (method == HttpMethod.Post || method == HttpMethod.Put) {
        request.SerializeData<T>(data);
      }

      var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();

      if (response.StatusCode == System.Net.HttpStatusCode.OK) {
        return await response.ReadContentAs<T>();
      } else {
        return default;
      }
    }
  }
}
