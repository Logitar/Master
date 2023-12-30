using Logitar.Master.Application.Projects;
using Logitar.Master.Contracts.Projects;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application;

/// <summary>
/// Provides extension methods for dependency injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers application services to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddLogitarMasterApplication(this IServiceCollection services)
  {
    return services
      .AddApplicationServices()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
  }

  private static IServiceCollection AddApplicationServices(this IServiceCollection services)
  {
    return services.AddTransient<IProjectService, ProjectService>();
  }
}
