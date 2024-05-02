using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts;
using MediatR;

namespace Logitar.Master.Domain.Projects.Events;

public record ProjectUpdatedEvent : DomainEvent, INotification
{
  public Change<DisplayNameUnit>? DisplayName { get; set; }
  public Change<DescriptionUnit>? Description { get; set; }

  public bool HasChanges => DisplayName != null || Description != null;
}
