using Logitar.EventSourcing;
using Logitar.Master.Contracts.Actors;

namespace Logitar.Master.EntityFrameworkCore.Relational.Actors;

internal interface IActorService
{
  Task<IEnumerable<Actor>> ResolveAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken = default);
}
