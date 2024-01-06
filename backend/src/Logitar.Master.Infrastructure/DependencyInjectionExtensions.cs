using Logitar.Identity.Infrastructure;
using Logitar.Master.Application;
using Logitar.Master.Application.Caching;
using Logitar.Master.Infrastructure.Caching;
using Logitar.Master.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarIdentityInfrastructure()
      .AddLogitarMasterApplication()
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton(serviceProvider =>
      {
        IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
        return configuration.GetSection("Cache").Get<CacheSettings>() ?? new();
      });
  }
}
