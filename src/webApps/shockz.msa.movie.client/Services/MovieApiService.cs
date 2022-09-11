using shockz.msa.movie.client.Models;

namespace shockz.msa.movie.client.Services
{
  public class MovieApiService : IMovieApiService
  {
    public async Task<IEnumerable<Movie>> GetMovies()
    {
      var movieList = new List<Movie>();
      movieList.Add(
        new Movie
        {
          Id = 1,
          Genre = "Comics",
          Title = "asd",
          Rating = "9.2",
          ImageUrl = "images/src",
          ReleaseDate = DateTime.Now,
          Owner = "shockz"
        });

      return await Task.FromResult(movieList);
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
