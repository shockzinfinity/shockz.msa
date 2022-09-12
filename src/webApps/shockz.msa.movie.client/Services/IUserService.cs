using shockz.msa.movie.client.Models;

namespace shockz.msa.movie.client.Services
{
  public interface IUserService
  {
    Task<UserInfoViewModel> GetUserInfo();
  }
}
