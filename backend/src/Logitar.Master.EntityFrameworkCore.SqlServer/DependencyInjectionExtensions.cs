using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Logitar.Master.EntityFrameworkCore.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.EntityFrameworkCore.SqlServer;

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
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionString = configuration.GetValue<string>("SQLCONNSTR_Master") ?? string.Empty;

    return services.AddLogitarMasterWithEntityFrameworkCoreSqlServer(connectionString);
  }
  /// <summary>
  /// Registers persistence services to the specified service collection.
  /// </summary>
  /// <param name="services">The service collection.</param>
  /// <param name="connectionString">The connection string.</param>
  /// <returns>The service collection.</returns>
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<MasterContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Logitar.Master.EntityFrameworkCore.SqlServer")))
      .AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString)
      .AddLogitarMasterWithEntityFrameworkCoreRelational()
      .AddSingleton<ISqlHelper, SqlServerHelper>();
  }
}
