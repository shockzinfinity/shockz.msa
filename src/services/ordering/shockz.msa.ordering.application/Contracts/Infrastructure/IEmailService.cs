using shockz.msa.ordering.application.Models;

namespace shockz.msa.ordering.application.Contracts.Infrastructure
{
  public interface IEmailService
  {
    Task<bool> SendEmail(Email email);
  }
}
