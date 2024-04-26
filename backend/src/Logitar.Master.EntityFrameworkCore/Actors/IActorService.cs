using Logitar.EventSourcing;
using Logitar.Master.Contracts.Actors;

namespace Logitar.Master.EntityFrameworkCore.Actors;

public interface IActorService
{
  Task<IReadOnlyCollection<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}
