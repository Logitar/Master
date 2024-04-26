using Logitar.EventSourcing;
using Logitar.Master.Domain.Shared;
using MediatR;

namespace Logitar.Master.Domain.Projects.Events;

public record ProjectUpdatedEvent : DomainEvent, INotification
{
  public Change<DisplayNameUnit>? DisplayName { get; set; }
  public Change<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => DisplayName != null || Description != null;
}
