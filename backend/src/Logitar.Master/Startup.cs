using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Master.Application;
using Logitar.Master.Authentication;
using Logitar.Master.Authorization;
using Logitar.Master.Constants;
using Logitar.Master.EntityFrameworkCore.SqlServer;
using Logitar.Master.Extensions;
using Logitar.Master.Filters;
using Logitar.Master.Middlewares;
using Logitar.Master.Settings;
using Microsoft.AspNetCore.Authorization;

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

    CookiesSettings cookiesSettings = _configuration.GetSection("Cookies").Get<CookiesSettings>() ?? new();
    services.AddSingleton(cookiesSettings);

    services
      .AddControllers(options =>
      {
        options.Filters.Add<ExceptionHandlingFilterAttribute>();
        //options.Filters.Add<LoggingFilterAttribute>(); // TODO(fpion): Logging
      })
      .AddJsonOptions(options =>
      {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
      });

    services.AddCors(_configuration);

    services.AddAuthentication()
      .AddScheme<SessionAuthenticationOptions, SessionAuthenticationHandler>(Schemes.Session, options => { });

    services.AddAuthorizationBuilder()
      .SetDefaultPolicy(new AuthorizationPolicyBuilder(Schemes.All)
        .RequireAuthenticatedUser()
        .Build()
      )
      .AddPolicy(Policies.SystemUser, new AuthorizationPolicyBuilder(Schemes.All)
        .RequireAuthenticatedUser()
        .AddRequirements(new SystemUserAuthorizationRequirement())
        .Build()
      );

    services.AddSession(options =>
    {
      options.Cookie.SameSite = cookiesSettings.Session.SameSite;
      options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    });

    if (_enableOpenApi)
    {
      services.AddOpenApi();
    }

    services.AddApplicationInsightsTelemetry();
    IHealthChecksBuilder healthChecks = services.AddHealthChecks();

    services.AddDistributedMemoryCache();
    services.AddMemoryCache();
    services.AddSingleton<IApplicationContext, HttpApplicationContext>();
    services.AddSingleton<IAuthorizationHandler, SystemUserAuthorizationHandler>();

    DatabaseProvider databaseProvider = _configuration.GetValue<DatabaseProvider?>("DatabaseProvider")
      ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        services.AddLogitarMasterWithEntityFrameworkCoreSqlServer(_configuration);
        healthChecks.AddDbContextCheck<EventContext>();
        healthChecks.AddDbContextCheck<IdentityContext>();
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
    builder.UseSession();
    // TODO(fpion): Logging
    builder.UseMiddleware<RenewSession>();
    builder.UseAuthentication();
    builder.UseAuthorization();

    if (builder is WebApplication application)
    {
      application.MapControllers();
      application.MapHealthChecks("/health");
    }
  }
}
