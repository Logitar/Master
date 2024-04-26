using Logitar.Master.Infrastructure.Commands;
using MediatR;

namespace Logitar.Master;

internal class Program
{
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    using IServiceScope scope = application.Services.CreateScope();
    IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    startup.Configure(application);

    application.Run();
  }
}
