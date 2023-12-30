using Logitar.EventSourcing.EntityFrameworkCore.PostgreSQL;
using Logitar.Master.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.EntityFrameworkCore.PostgreSQL;

/// <summary>
/// Provides extension methods for dependency injection.
/// </summary>
public static class DependencyInjectionExtensions
{
  /// <summary>
  /// Registers persistence services to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="configuration">The application configuration.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionString = configuration.GetValue<string>("POSTGRESQLCONNSTR_Master") ?? string.Empty;

    return services.AddLogitarMasterWithEntityFrameworkCorePostgreSQL(connectionString);
  }
  /// <summary>
  /// Registers persistence services to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="connectionString">The connection string.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCorePostgreSQL(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<MasterContext>(options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Logitar.Master.EntityFrameworkCore.PostgreSQL")))
      .AddLogitarEventSourcingWithEntityFrameworkCorePostgreSQL(connectionString)
      .AddLogitarMasterWithEntityFrameworkCoreRelational()
      .AddSingleton<ISqlHelper, PostgresHelper>();
  }
}
