using Bogus;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Master.EntityFrameworkCore;
using Logitar.Master.EntityFrameworkCore.SqlServer;
using Logitar.Master.Infrastructure;
using Logitar.Master.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly DatabaseProvider _databaseProvider;

  protected Faker Faker { get; } = new();

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected EventContext EventContext { get; }
  protected MasterContext MasterContext { get; }

  protected IMediator Mediator { get; }

  protected IntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);

    string connectionString;
    _databaseProvider = Configuration.GetValue<DatabaseProvider?>("DatabaseProvider") ?? DatabaseProvider.EntityFrameworkCoreSqlServer;
    switch (_databaseProvider)
    {
      case DatabaseProvider.EntityFrameworkCoreSqlServer:
        connectionString = Configuration.GetValue<string>("SQLCONNSTR_Master")?.Replace("{Database}", GetType().Name) ?? string.Empty;
        services.AddLogitarMasterWithEntityFrameworkCoreSqlServer(connectionString);
        break;
      default:
        throw new DatabaseProviderNotSupportedException(_databaseProvider);
    }

    ServiceProvider = services.BuildServiceProvider();

    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    MasterContext = ServiceProvider.GetRequiredService<MasterContext>();

    Mediator = ServiceProvider.GetRequiredService<IMediator>();
  }

  public virtual async Task InitializeAsync()
  {
    await Mediator.Publish(new InitializeDatabaseCommand());

    StringBuilder command = new();
    command.AppendLine(CreateDeleteBuilder(MasterDb.Projects.Table).Build().Text);
    command.AppendLine(CreateDeleteBuilder(MasterDb.Actors.Table).Build().Text);
    command.AppendLine(CreateDeleteBuilder(EventDb.Events.Table).Build().Text);
    await MasterContext.Database.ExecuteSqlRawAsync(command.ToString());
  }
  private IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.EntityFrameworkCoreSqlServer => SqlServerDeleteBuilder.From(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public virtual Task DisposeAsync() => Task.CompletedTask;
}
