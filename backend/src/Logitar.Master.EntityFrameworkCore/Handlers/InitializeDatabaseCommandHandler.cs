using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Master.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Master.EntityFrameworkCore.Handlers;

internal class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly bool _enableMigrations;
  private readonly EventContext _eventContext;
  private readonly MasterContext _masterContext;

  public InitializeDatabaseCommandHandler(IConfiguration configuration, EventContext eventContext, MasterContext masterContext)
  {
    _enableMigrations = configuration.GetValue<bool>("EnableMigrations");
    _eventContext = eventContext;
    _masterContext = masterContext;
  }

  public async Task Handle(InitializeDatabaseCommand _, CancellationToken cancellationToken)
  {
    if (_enableMigrations)
    {
      await _eventContext.Database.MigrateAsync(cancellationToken);
      await _masterContext.Database.MigrateAsync(cancellationToken);
    }
  }
}
