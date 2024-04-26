using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Master.Application.Projects;
using Logitar.Master.Domain.Projects;
using Logitar.Master.EntityFrameworkCore.Actors;
using Logitar.Master.EntityFrameworkCore.Queriers;
using Logitar.Master.EntityFrameworkCore.Relational.Actors;
using Logitar.Master.EntityFrameworkCore.Repositories;
using Logitar.Master.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.EntityFrameworkCore;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCore(this IServiceCollection services)
  {
    return services
      .AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
      .AddLogitarEventSourcingWithEntityFrameworkCoreRelational()
      .AddLogitarMasterInfrastructure()
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
