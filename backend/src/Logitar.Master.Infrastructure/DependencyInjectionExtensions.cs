using Logitar.EventSourcing.Infrastructure;
using Logitar.Identity.Infrastructure.Converters;
using Logitar.Master.Application;
using Logitar.Master.Application.Accounts;
using Logitar.Master.Application.Caching;
using Logitar.Master.Infrastructure.Caching;
using Logitar.Master.Infrastructure.Converters;
using Logitar.Master.Infrastructure.IdentityServices;
using Logitar.Master.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddLogitarMasterApplication()
      .AddIdentityServices()
      .AddMemoryCache()
      .AddSingleton(InitializeCachingSettings)
      .AddSingleton<ICacheService, CacheService>()
      .AddSingleton<IEventSerializer>(BuildEventSerializer)
      .AddTransient<IEventBus, EventBus>();
  }

  private static IServiceCollection AddIdentityServices(this IServiceCollection services)
  {
    return services
      .AddTransient<IApiKeyService, ApiKeyService>()
      .AddTransient<IMessageService, MessageService>()
      .AddTransient<IOneTimePasswordService, OneTimePasswordService>()
      .AddTransient<ISessionService, SessionService>()
      .AddTransient<ITokenService, TokenService>()
      .AddTransient<IUserService, UserService>();
  }

  private static EventSerializer BuildEventSerializer(IServiceProvider serviceProvider)
  {
    EventSerializer eventSerializer = new();

    eventSerializer.RegisterConverter(new DescriptionConverter());
    eventSerializer.RegisterConverter(new DisplayNameConverter());
    eventSerializer.RegisterConverter(new UniqueKeyConverter());
    eventSerializer.RegisterConverter(new ProjectIdConverter());

    return eventSerializer;
  }

  private static CachingSettings InitializeCachingSettings(IServiceProvider serviceProvider)
  {
    IConfiguration configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return configuration.GetSection("Caching").Get<CachingSettings>() ?? new();
  }
}
