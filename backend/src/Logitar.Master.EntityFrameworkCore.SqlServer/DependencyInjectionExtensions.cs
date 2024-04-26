using Logitar.EventSourcing.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  private const string ConfigurationKey = "SQLCONNSTR_Master";

  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string? connectionString = Environment.GetEnvironmentVariable(ConfigurationKey);
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      connectionString = configuration.GetValue<string>(ConfigurationKey);
    }
    if (string.IsNullOrWhiteSpace(connectionString))
    {
      throw new ArgumentException($"The configuration '{ConfigurationKey}' could not be found.", nameof(configuration));
    }
    return services.AddLogitarMasterWithEntityFrameworkCoreSqlServer(connectionString.Trim());
  }
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddDbContext<MasterContext>(options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Logitar.Master.EntityFrameworkCore.SqlServer")))
      .AddLogitarEventSourcingWithEntityFrameworkCoreSqlServer(connectionString)
      .AddLogitarMasterWithEntityFrameworkCore()
      .AddSingleton<ISqlHelper, SqlServerHelper>();
  }
}
