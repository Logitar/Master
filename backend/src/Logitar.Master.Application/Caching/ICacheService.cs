using Logitar.EventSourcing;
using Logitar.Master.Contracts.Actors;

namespace Logitar.Master.Application.Caching;

/// <summary>
/// Defines methods to manage caching.
/// </summary>
public interface ICacheService
{
  /// <summary>
  /// Tries retrieving the specified actor from the cache.
  /// </summary>
  /// <param name="id">The identifier of the actor.</param>
  /// <returns>The retrieved actor, or null.</returns>
  Actor? GetActor(ActorId id);
  /// <summary>
  /// Stores or removes (if null) the actor from the cache.
  /// </summary>
  /// <param name="id">The identifier of the actor.</param>
  /// <param name="actor">The actor.</param>
  void SetActor(ActorId id, Actor? actor);
}
