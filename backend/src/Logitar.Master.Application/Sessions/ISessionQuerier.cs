using Logitar.Identity.Domain.Sessions;
using Logitar.Master.Contracts.Sessions;

namespace Logitar.Master.Application.Sessions;

public interface ISessionQuerier
{
  Task<Session> ReadAsync(SessionAggregate session, CancellationToken cancellationToken = default);
  Task<Session?> ReadAsync(string id, CancellationToken cancellationToken = default);
}
