using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Master.Application.Sessions;
using Logitar.Master.EntityFrameworkCore.Relational.Queriers;
using Logitar.Master.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.EntityFrameworkCore.Relational;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityWithEntityFrameworkCoreRelational()
      .AddLogitarMasterInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services.AddTransient<ISessionQuerier, SessionQuerier>();
  }
}
