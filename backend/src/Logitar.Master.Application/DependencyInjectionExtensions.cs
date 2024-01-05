using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Settings;
using Logitar.Master.Application.Account;
using Logitar.Master.Contracts.Account;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterApplication(this IServiceCollection services)
  {
    return services
      .AddApplicationServices()
      .AddLogitarIdentityDomain()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddSingleton<IUserSettings>(serviceProvider =>
      {
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetSection("User").Get<UserSettings>() ?? new();
      });
  }

  private static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    return services.AddTransient<IAccountService, AccountService>();
  }
}
