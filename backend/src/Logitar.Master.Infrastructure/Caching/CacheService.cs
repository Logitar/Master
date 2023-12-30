using Logitar.EventSourcing;
using Logitar.Master.Application.Caching;
using Logitar.Master.Contracts.Actors;
using Logitar.Master.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Master.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _cache;

  private TimeSpan? ActorExpiration { get; }

  public CacheService(IMemoryCache cache, CacheSettings settings)
  {
    _cache = cache;

    if (settings.ActorExpirationSeconds > 0)
    {
      ActorExpiration = TimeSpan.FromSeconds(settings.ActorExpirationSeconds);
    }
  }

  public Actor? GetActor(ActorId id) => GetItem<Actor>(GetActorKey(id));
  public void SetActor(ActorId id, Actor? actor)
  {
    string key = GetActorKey(id);
    if (actor == null)
    {
      RemoveItem(key);
    }
    else
    {
      SetItem(key, actor, ActorExpiration);
    }
  }
  private static string GetActorKey(ActorId id) => $"Actor.Id:{id}";

  private T? GetItem<T>(object key) => _cache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void RemoveItem(object key) => _cache.Remove(key);
  private void SetItem<T>(object key, T? value, TimeSpan? expiration = null)
  {
    if (expiration.HasValue)
    {
      _cache.Set(key, value, expiration.Value);
    }
    else
    {
      _cache.Set(key, value);
    }
  }
}
