using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Master.Domain.Projects.Events;

/// <summary>
/// The event raised when the unique key of a project is changed.
/// </summary>
public record ProjectUniqueKeyChangedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Gets the new unique key of the project.
  /// </summary>
  public UniqueKeyUnit UniqueKey { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ProjectUniqueKeyChangedEvent"/> class.
  /// </summary>
  /// <param name="actorId">The identifier of the actor who changed the project unique key.</param>
  /// <param name="uniqueKey">The new unique key of the project.</param>
  public ProjectUniqueKeyChangedEvent(ActorId actorId, UniqueKeyUnit uniqueKey)
  {
    ActorId = actorId;
    UniqueKey = uniqueKey;
  }
}
