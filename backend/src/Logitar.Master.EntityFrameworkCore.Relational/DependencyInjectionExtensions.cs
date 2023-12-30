using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Master.Application.Projects;
using Logitar.Master.Domain.Projects;
using Logitar.Master.EntityFrameworkCore.Relational.Actors;
using Logitar.Master.EntityFrameworkCore.Relational.Queriers;
using Logitar.Master.EntityFrameworkCore.Relational.Repositories;
using Logitar.Master.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.EntityFrameworkCore.Relational;

/// <summary>
/// Provides extension methods for dependency injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers persistence services to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCoreRelational(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddLogitarMasterInfrastructure()
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddQueriers()
      .AddRepositories()
      .AddTransient<IActorService, ActorService>();
  }

  private static IServiceCollection AddQueriers(this IServiceCollection services)
  {
    return services.AddTransient<IProjectQuerier, ProjectQuerier>();
  }

  private static IServiceCollection AddRepositories(this IServiceCollection services)
  {
    return services.AddTransient<IProjectRepository, ProjectRepository>();
  }
}
