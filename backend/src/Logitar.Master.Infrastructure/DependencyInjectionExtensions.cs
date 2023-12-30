using Logitar.EventSourcing.Infrastructure;
using Logitar.Master.Application;
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
      .AddLogitarMasterApplication();
  }
}
