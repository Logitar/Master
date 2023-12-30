using Logitar.EventSourcing;
using Logitar.Master.Application.Caching;
using Logitar.Master.Contracts.Actors;
using Logitar.Master.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Actors;

internal class ActorService : IActorService
{
  private readonly DbSet<ActorEntity> _actors;
  private readonly ICacheService _cacheService;

  public ActorService(ICacheService cacheService, MasterContext context)
  {
    _actors = context.Actors;
    _cacheService = cacheService;
  }

  public async Task<IEnumerable<Actor>> ResolveAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    List<Actor> actors = new(capacity: ids.Count());
    List<string> misses = new(capacity: actors.Capacity);

    foreach (ActorId id in ids)
    {
      Actor? actor = _cacheService.GetActor(id);
      if (actor == null)
      {
        misses.Add(id.Value);
      }
      else
      {
        actors.Add(actor);
      }
    }

    if (misses.Count > 0)
    {
      ActorEntity[] entities = await _actors.AsNoTracking()
        .Where(a => misses.Contains(a.Id))
        .ToArrayAsync(cancellationToken);

      actors.AddRange(entities.Select(entity => entity.ToActor()));
    }

    foreach (Actor actor in actors)
    {
      ActorId id = new(actor.Id);
      _cacheService.SetActor(id, actor);
    }

    return actors.AsReadOnly();
  }
}
