using Logitar.Identity.Infrastructure;
using Logitar.Master.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityInfrastructure()
      .AddLogitarMasterApplication();
  }
}
