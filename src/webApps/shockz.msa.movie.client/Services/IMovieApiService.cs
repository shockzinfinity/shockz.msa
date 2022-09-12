using shockz.msa.movie.client.Models;

namespace shockz.msa.movie.client.Services
{
  public interface IMovieApiService
  {
    Task<IEnumerable<Movie>> GetMovies();
    Task<Movie> GetMovie(int id);
    Task<bool> CreateMovie(Movie movie);
    Task<bool> UpdateMovie(int id, Movie movie);
    Task<bool> DeleteMovice(int id);
  }
}
