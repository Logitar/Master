using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Master.Application.Sessions;
using Logitar.Master.Contracts.Actors;
using Logitar.Master.Contracts.Sessions;
using Logitar.Master.EntityFrameworkCore.Relational.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly IActorService _actorService;
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IActorService actorService, IdentityContext context)
  {
    _actorService = actorService;
    _sessions = context.Sessions;
  }

  public async Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken)
  {
    return await ReadAsync(session.Id.Value, cancellationToken)
      ?? throw new InvalidOperationException($"The session 'AggregateId={session.Id.Value}' could not be found.");
  }
  public async Task<Session?> ReadAsync(string id, CancellationToken cancellationToken)
  {
    SessionEntity? session = await _sessions.AsNoTracking()
      .Include(x => x.User)
      .SingleOrDefaultAsync(x => x.AggregateId == id, cancellationToken);

    return session == null ? null : await MapAsync(session, cancellationToken);
  }

  private async Task<Session> MapAsync(SessionEntity session, CancellationToken cancellationToken)
    => (await MapAsync([session], cancellationToken)).Single();
  private async Task<IEnumerable<Session>> MapAsync(IEnumerable<SessionEntity> sessions, CancellationToken cancellationToken)
  {
    IEnumerable<ActorId> actorIds = sessions.SelectMany(session => session.GetActorIds());
    IEnumerable<Actor> actors = await _actorService.FindAsync(actorIds, cancellationToken);
    Mapper mapper = new(actors);

    return sessions.Select(mapper.ToSession);
  }
}
