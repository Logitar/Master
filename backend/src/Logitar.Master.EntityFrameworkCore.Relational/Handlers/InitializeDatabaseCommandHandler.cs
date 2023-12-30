using Logitar.EventSourcing.EntityFrameworkCore.Relational;
using Logitar.Master.Infrastructure.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Master.EntityFrameworkCore.Relational.Handlers;

internal class InitializeDatabaseCommandHandler : INotificationHandler<InitializeDatabaseCommand>
{
  private readonly IConfiguration _configuration;
  private readonly EventContext _eventContext;
  private readonly MasterContext _masterContext;

  public InitializeDatabaseCommandHandler(IConfiguration configuration, EventContext eventContext, MasterContext masterContext)
  {
    _configuration = configuration;
    _eventContext = eventContext;
    _masterContext = masterContext;
  }

  public async Task Handle(InitializeDatabaseCommand _, CancellationToken cancellationToken)
  {
    if (_configuration.GetValue<bool>("EnableMigrations"))
    {
      await _eventContext.Database.MigrateAsync(cancellationToken);
      await _masterContext.Database.MigrateAsync(cancellationToken);
    }
  }
}
