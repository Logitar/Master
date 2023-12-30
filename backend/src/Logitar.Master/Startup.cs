using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Master.Application;
using Logitar.Master.EntityFrameworkCore.PostgreSQL;
using Logitar.Master.EntityFrameworkCore.Relational;
using Logitar.Master.EntityFrameworkCore.SqlServer;
using Logitar.Master.Extensions;
using Logitar.Master.Settings;
using System.Text.Json.Serialization;

namespace Logitar.Master;

internal class Startup : StartupBase
{
  private readonly IConfiguration _configuration;
  private readonly bool _enableOpenApi;

  public Startup(IConfiguration configuration)
  {
    _configuration = configuration;
    _enableOpenApi = configuration.GetValue<bool>("EnableOpenApi");
  }

  public override void ConfigureServices(IServiceCollection services)
  {
    base.ConfigureServices(services);

    services.AddControllers()
      .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

    CorsSettings corsSettings = _configuration.GetSection("Cors").Get<CorsSettings>() ?? new();
    services.AddSingleton(corsSettings);
    services.AddCors(corsSettings);

    //services.AddAuthentication()
    //  .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(Schemes.ApiKey, options => { })
    //  .AddScheme<BasicAuthenticationOptions, BasicAuthenticationHandler>(Schemes.Basic, options => { })
    //  .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { }); // TODO(fpion): Authentication

    //services.AddAuthorization(options =>
    //{
    //  options.AddPolicy(Policies.PortalActor, new AuthorizationPolicyBuilder(Schemes.All)
    //    .RequireAuthenticatedUser()
    //    .AddRequirements(new PortalActorAuthorizationRequirement())
    //    .Build());
    //}); // TODO(fpion): Authorization

    //CookiesSettings cookiesSettings = configuration.GetSection("Cookies").Get<CookiesSettings>() ?? new();
    //services.AddSingleton(cookiesSettings);
    //services.AddSession(options =>
    //{
    //  options.Cookie.SameSite = cookiesSettings.Session.SameSite;
    //  options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    //}); // TODO(fpion): Session

    services.AddApplicationInsightsTelemetry();
    services.AddMemoryCache();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCorePostgreSQL:
        services.AddLogitarMasterWithEntityFrameworkCorePostgreSQL(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<MasterContext>();
        break;
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarMasterWithEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<MasterContext>();
        break;
      default:
        throw new DatabaseProviderNotSupportedException(databaseProvider);
    }
  }

  public override void Configure(IApplicationBuilder builder)
  {
    if (_enableOpenApi)
    {
      builder.UseOpenApi();
    }

    builder.UseHttpsRedirection();
    builder.UseCors();
    builder.UseAuthentication();
    builder.UseAuthorization();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
