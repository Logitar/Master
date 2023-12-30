using Logitar.Master.Infrastructure.Commands;
using MediatR;

namespace Logitar.Master;

/// <summary>
/// The main class of the application.
/// </summary>
public class Program
{
  /// <summary>
  /// The main entry point of the application.
  /// </summary>
  /// <param name="args">The command-line arguments.</param>
  /// <returns>The asynchronous operation.</returns>
  public static async Task Main(string[] args)
  {
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    Startup startup = new(builder.Configuration);
    startup.ConfigureServices(builder.Services);

    WebApplication application = builder.Build();

    startup.Configure(application);

    using IServiceScope scope = application.Services.CreateScope();
    IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    application.Run();
  }
}
