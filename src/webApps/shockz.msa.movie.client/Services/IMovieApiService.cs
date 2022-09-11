using shockz.msa.movie.client.Models;

namespace shockz.msa.movie.client.Services
{
  public interface IMovieApiService
  {
    Task<IEnumerable<Movie>> GetMovies();
    Task<Movie> GetMovie(int id);
    Task<Movie> CreateMovie(Movie movie);
    Task<Movie> UpdateMovie(Movie movie);
    Task DeleteMovice(int id);
    Task<UserInfoViewModel> GetUserInfo();
  }
}
