using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterApplication(this IServiceCollection services)
  {
    return services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }
}
