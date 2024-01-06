using Logitar.Identity.Domain;
using Logitar.Master.Application.Account;
using Logitar.Master.Application.Sessions;
using Logitar.Master.Contracts.Account;
using Logitar.Master.Contracts.Sessions;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterApplication(this IServiceCollection services)
  {
    return services
      .AddApplicationServices()
      .AddLogitarIdentityDomain()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }

  private static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IAccountService, AccountService>()
      .AddTransient<ISessionService, SessionService>();
  }
}
