using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Master.Domain.Projects.Events;

/// <summary>
/// The event raised when a project is deleted.
/// </summary>
public record ProjectDeletedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Initializes a new instance of the <see cref="ProjectDeletedEvent"/> class.
  /// </summary>
  /// <param name="actorId">The identifier of the actor who deleted the project.</param>
  public ProjectDeletedEvent(ActorId actorId)
  {
    ActorId = actorId;
    IsDeleted = true;
  }
}
