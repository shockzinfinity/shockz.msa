using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using shockz.msa.ordering.application.Behaviors;
using System.Reflection;

namespace shockz.msa.ordering.application
{
  public static class ApplicationServiceRegistration
  {
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
      services.AddAutoMapper(Assembly.GetExecutingAssembly());
      services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
      services.AddMediatR(Assembly.GetExecutingAssembly());

      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
      services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

      return services;
    }
  }
}
