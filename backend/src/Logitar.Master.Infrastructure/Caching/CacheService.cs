using Logitar.EventSourcing;
using Logitar.Master.Application.Caching;
using Logitar.Master.Contracts.Actors;
using Logitar.Master.Infrastructure.Settings;
using Microsoft.Extensions.Caching.Memory;

namespace Logitar.Master.Infrastructure.Caching;

internal class CacheService : ICacheService
{
  private readonly IMemoryCache _memoryCache;
  private readonly CacheSettings _settings;

  public CacheService(IMemoryCache memoryCache, CacheSettings settings)
  {
    _memoryCache = memoryCache;
    _settings = settings;
  }

  public Actor? GetActor(ActorId id) => GetItem<Actor>(GetActorKey(id));
  public void SetActor(Actor actor)
  {
    TimeSpan? expiration = _settings.ActorExpiration == null ? null : TimeSpan.Parse(_settings.ActorExpiration);
    SetItem(GetActorKey(actor.Id), actor, expiration);
  }
  private static string GetActorKey(string id) => GetActorKey(new ActorId(id));
  private static string GetActorKey(ActorId id) => $"Actor.Id|{id}";

  private T? GetItem<T>(object key) => _memoryCache.TryGetValue(key, out object? value) ? (T?)value : default;
  private void SetItem<T>(object key, T value, TimeSpan? expiration = null)
  {
    if (expiration.HasValue)
    {
      _memoryCache.Set(key, value, expiration.Value);
    }
    else
    {
      _memoryCache.Set(key, value);
    }
  }
}
