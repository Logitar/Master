using Logitar.EventSourcing.Infrastructure;
using Logitar.Master.Application;
using Logitar.Master.Infrastructure.Converters;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Infrastructure;

public static class DependencyInjectionExtensions
{
  public static IServiceCollection AddLogitarMasterInfrastructure(this IServiceCollection services)
  {
    return services
      .AddLogitarEventSourcingInfrastructure()
      .AddLogitarMasterApplication()
      .AddSingleton<IEventSerializer>(BuildEventSerializer);
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
}
