using Logitar.EventSourcing;
using Logitar.Identity.EntityFrameworkCore.Relational;
using Logitar.Identity.EntityFrameworkCore.Relational.Entities;
using Logitar.Master.Application.Caching;
using Logitar.Master.Contracts.Actors;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Master.EntityFrameworkCore.Relational.Actors;

internal class ActorService : IActorService
{
  private readonly DbSet<ActorEntity> _actors;
  private readonly ICacheService _cacheService;

  public ActorService(ICacheService cacheService, IdentityContext context)
  {
    _actors = context.Actors;
    _cacheService = cacheService;
  }

  public async Task<IEnumerable<Actor>> FindAsync(IEnumerable<ActorId> ids, CancellationToken cancellationToken)
  {
    int capacity = ids.Count();
    Dictionary<ActorId, Actor> actors = new(capacity);
    HashSet<string> missingIds = new(capacity);

    foreach (ActorId id in ids)
    {
      if (id != default)
      {
        Actor? actor = _cacheService.GetActor(id);
        if (actor == null)
        {
          missingIds.Add(id.Value);
        }
        else
        {
          actors[id] = actor;
          _cacheService.SetActor(actor);
        }
      }
    }

    if (missingIds.Count > 0)
    {
      Mapper mapper = new();

      ActorEntity[] entities = await _actors.AsNoTracking()
        .Where(a => missingIds.Contains(a.Id))
        .ToArrayAsync(cancellationToken);

      foreach (ActorEntity entity in entities)
      {
        ActorId id = new(entity.Id);
        Actor actor = mapper.ToActor(entity);

        actors[id] = actor;
        _cacheService.SetActor(actor);
      }
    }

    return actors.Values;
  }
}
