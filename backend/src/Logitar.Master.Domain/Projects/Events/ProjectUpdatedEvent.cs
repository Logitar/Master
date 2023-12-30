using Logitar.EventSourcing;
using Logitar.Master.Contracts;
using MediatR;

namespace Logitar.Master.Domain.Projects.Events;

/// <summary>
/// The event raised when a project is updated.
/// </summary>
public record ProjectUpdatedEvent : DomainEvent, INotification
{
  /// <summary>
  /// Gets or sets the display name of the project.
  /// </summary>
  public Modification<DisplayNameUnit>? DisplayName { get; set; }
  /// <summary>
  /// Gets or sets the description of the project.
  /// </summary>
  public Modification<DescriptionUnit>? Description { get; set; }

  /// <summary>
  /// Gets a value indicating whether or not the event has changes.
  /// </summary>
  public bool HasChanges => DisplayName != null || Description != null;
}
