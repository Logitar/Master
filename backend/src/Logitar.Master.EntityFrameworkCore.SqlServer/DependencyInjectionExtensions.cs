using Logitar.Identity.EntityFrameworkCore.SqlServer;
using Logitar.Master.EntityFrameworkCore.Relational;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.EntityFrameworkCore.SqlServer;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCoreSqlServer(this IServiceCollection services, IConfiguration configuration)
  {
    string connectionString = configuration.GetValue<string>("SQLCONNSTR_Master") ?? string.Empty;
    return services.AddLogitarMasterWithEntityFrameworkCoreSqlServer(connectionString);
  }

  public static IServiceCollection AddLogitarMasterWithEntityFrameworkCoreSqlServer(this IServiceCollection services, string connectionString)
  {
    return services
      .AddLogitarIdentityWithEntityFrameworkCoreSqlServer(connectionString)
      .AddLogitarMasterWithEntityFrameworkCoreRelational();
  }
}
