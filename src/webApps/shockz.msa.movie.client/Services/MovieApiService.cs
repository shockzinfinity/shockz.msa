using shockz.msa.movie.client.Models;
using System.Text.Json;

namespace shockz.msa.movie.client.Services
{
  public class MovieApiService : IMovieApiService
  {
    private readonly IHttpClientFactory _httpClientFactory;

    public MovieApiService(IHttpClientFactory httpClientFactory)
    {
      _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
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

      var httpClient = _httpClientFactory.CreateClient("MovieAPIClient");

      var request = new HttpRequestMessage(HttpMethod.Get, "/api/movies/");

      var movies = await httpClient.GetFromJsonAsync<IEnumerable<Movie>>("/api/movies/");

      return movies;
    }

    public Task<Movie> GetMovie(int id)
    {
      throw new NotImplementedException();
    }

    public Task<Movie> CreateMovie(Movie movie)
    {
      throw new NotImplementedException();
    }

    public Task<Movie> UpdateMovie(Movie movie)
    {
      throw new NotImplementedException();
    }

    public Task DeleteMovice(int id)
    {
      throw new NotImplementedException();
    }
  }
}
