using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Master.Domain.Projects.Events;

/// <summary>
/// The event raised when a new project is created.
/// </summary>
public record ProjectCreatedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Gets the unique key of the project.
  /// </summary>
  public UniqueKeyUnit UniqueKey { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ProjectCreatedEvent"/> class.
  /// </summary>
  /// <param name="actorId">The identifier of the actor who created the project.</param>
  /// <param name="uniqueKey">The unique key of the project.</param>
  public ProjectCreatedEvent(ActorId actorId, UniqueKeyUnit uniqueKey)
  {
    ActorId = actorId;
    UniqueKey = uniqueKey;
  }
}
