using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using shockz.msa.ordering.application.Contracts.Infrastructure;
using shockz.msa.ordering.application.Contracts.Persistence;
using shockz.msa.ordering.application.Models;
using shockz.msa.ordering.infrastructure.Email;
using shockz.msa.ordering.infrastructure.Persistence;
using shockz.msa.ordering.infrastructure.Repositories;

namespace shockz.msa.ordering.infrastructure;

public static class InfrastructureServiceRegistration
{
  public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddDbContext<OrderContext>(options => options.UseSqlServer(configuration.GetConnectionString("OrderingConnectionString")));

    // for the meaditor
    services.AddScoped(typeof(IAsyncRepository<>), typeof(RepositoryBase<>));
    services.AddScoped<IOrderingRepository, OrderingRepository>();

    services.Configure<EmailSettings>(c => configuration.GetSection("EmailSettings"));
    services.AddTransient<IEmailService, EmailService>();

    return services;
  }
}
