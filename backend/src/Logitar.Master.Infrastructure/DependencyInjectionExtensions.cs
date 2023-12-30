using Logitar.EventSourcing.Infrastructure;
using Logitar.Master.Application;
using Logitar.Master.Application.Caching;
using Logitar.Master.Infrastructure.Caching;
using Logitar.Master.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Infrastructure;

/// <summary>
/// Provides extension methods for dependency injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers infrastructure services to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddLogitarMasterInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddLogitarMasterApplication()
      .AddSingleton(provider =>
      {
        IConfiguration configuration = provider.GetRequiredService<IConfiguration>();
        return configuration.GetSection("Cache").Get<CacheSettings>() ?? new();
      })
      .AddTransient<ICacheService, CacheService>()
      .AddTransient<IEventBus, EventBus>();
  }
}
