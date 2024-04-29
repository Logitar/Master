using Bogus;
using Logitar.Data;
using Logitar.Data.SqlServer;
using Logitar.EventSourcing;
using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Master.Application;
using Logitar.Master.Application.Accounts;
using Logitar.Master.EntityFrameworkCore;
using Logitar.Master.EntityFrameworkCore.Entities;
using Logitar.Master.EntityFrameworkCore.SqlServer;
using Logitar.Master.Infrastructure;
using Logitar.Master.Infrastructure.Commands;
using Logitar.Portal.Contracts.Actors;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Logitar.Master;

public abstract class IntegrationTests : IAsyncLifetime
{
  private readonly TestContext _context = new();
  private readonly DatabaseProvider _databaseProvider;

  protected CancellationToken CancellationToken { get; }
  protected Faker Faker { get; } = new();

  protected IConfiguration Configuration { get; }
  protected IServiceProvider ServiceProvider { get; }

  protected EventContext EventContext { get; }
  protected MasterContext MasterContext { get; }

  protected IRequestPipeline Pipeline { get; }

  protected Mock<IMessageService> MessageService { get; } = new();
  protected Mock<IOneTimePasswordService> OneTimePasswordService { get; } = new();
  protected Mock<ISessionService> SessionService { get; } = new();
  protected Mock<ITokenService> TokenService { get; } = new();
  protected Mock<IUserService> UserService { get; } = new();

  protected Actor Actor { get; } = Actor.System;
  protected ActorId ActorId => new(Actor.Id);

  protected IntegrationTests()
  {
    Configuration = new ConfigurationBuilder()
      .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
      .Build();

    ServiceCollection services = new();
    services.AddSingleton(Configuration);
    services.AddSingleton(_context);
    services.AddSingleton<IRequestPipeline, TestRequestPipeline>();

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

    services.AddSingleton(MessageService.Object);
    services.AddSingleton(OneTimePasswordService.Object);
    services.AddSingleton(SessionService.Object);
    services.AddSingleton(TokenService.Object);
    services.AddSingleton(UserService.Object);

    ServiceProvider = services.BuildServiceProvider();

    EventContext = ServiceProvider.GetRequiredService<EventContext>();
    MasterContext = ServiceProvider.GetRequiredService<MasterContext>();

    Pipeline = ServiceProvider.GetRequiredService<IRequestPipeline>();

    DateTime now = DateTime.Now;
    User user = new(Faker.Person.UserName)
    {
      Id = Guid.NewGuid(),
      Version = 1,
      CreatedOn = now,
      UpdatedOn = now,
      Email = new Email(Faker.Person.Email),
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      FullName = Faker.Person.FullName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString().ToLower(),
      Picture = Faker.Person.Avatar,
      Website = $"https://www.{Faker.Person.Website}"
    };
    Actor = new(user);
    user.CreatedBy = Actor;
    user.UpdatedBy = Actor;
    _context.User = user;
  }

  public virtual async Task InitializeAsync()
  {
    IPublisher publisher = ServiceProvider.GetRequiredService<IPublisher>();
    await publisher.Publish(new InitializeDatabaseCommand());

    StringBuilder command = new();
    command.AppendLine(CreateDeleteBuilder(MasterDb.Projects.Table).Build().Text);
    command.AppendLine(CreateDeleteBuilder(MasterDb.Actors.Table).Build().Text);
    command.AppendLine(CreateDeleteBuilder(EventDb.Events.Table).Build().Text);
    await MasterContext.Database.ExecuteSqlRawAsync(command.ToString());

    if (_context.User != null)
    {
      ActorEntity actor = new(_context.User);
      MasterContext.Actors.Add(actor);
      await MasterContext.SaveChangesAsync();
    }
  }
  private IDeleteBuilder CreateDeleteBuilder(TableId table) => _databaseProvider switch
  {
    DatabaseProvider.EntityFrameworkCoreSqlServer => SqlServerDeleteBuilder.From(table),
    _ => throw new DatabaseProviderNotSupportedException(_databaseProvider),
  };

  public virtual Task DisposeAsync() => Task.CompletedTask;
}
