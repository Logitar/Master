using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Master.Domain.Projects.Events;

public record ProjectDeletedEvent : DomainEvent, INotification;
