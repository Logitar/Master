using Logitar.EventSourcing;
using Logitar.Master.Application.Accounts.Events;
using Logitar.Master.EntityFrameworkCore.Entities;
using Logitar.Portal.Contracts.Users;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Handlers;

internal static class Accounts
{
  public class UserSignedInEventHandler : INotificationHandler<UserSignedInEvent>
  {
    private readonly MasterContext _context;

    public UserSignedInEventHandler(MasterContext context)
    {
      _context = context;
    }

    public async Task Handle(UserSignedInEvent @event, CancellationToken cancellationToken)
    {
      User user = @event.Session.User;

      string id = new ActorId(user.Id).Value;
      ActorEntity? actor = await _context.Actors.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
      if (actor == null)
      {
        actor = new(user);

        _context.Actors.Add(actor);
      }
      else
      {
        actor.Update(user);
      }

      await _context.SaveChangesAsync(cancellationToken);
    }
  }
}
