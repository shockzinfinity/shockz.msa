using System.Text.Json;

namespace shockz.msa.movie.client.Extensions
{
  public static class HttpClientExtensions
  {
    public static async Task<T> ReadContentAs<T>(this HttpResponseMessage response)
    {
      if (!response.IsSuccessStatusCode) {
        throw new ApplicationException($"Something went wrong calling the API: {response.ReasonPhrase}");
      }

      var dataAsString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
      return JsonSerializer.Deserialize<T>(dataAsString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public static void SerializeData<T>(this HttpRequestMessage request, T data)
    {
      var dataAsString = JsonSerializer.Serialize(data);
      var content = new StringContent(dataAsString);

      content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(shockz.msa.common.Constant.Content_Type_Json);
      request.Content = content;
    }
  }
}
