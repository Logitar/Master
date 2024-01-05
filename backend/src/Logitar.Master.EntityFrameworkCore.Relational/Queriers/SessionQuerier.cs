using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Master.Application.Sessions;
using Logitar.Master.Contracts.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Queriers;

internal class SessionQuerier : ISessionQuerier
{
  private readonly DbSet<SessionEntity> _sessions;

  public SessionQuerier(IdentityContext context)
  {
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
    Mapper mapper = new();

    return sessions.Select(mapper.ToSession);
  }
}
