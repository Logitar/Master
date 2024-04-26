using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Master.Domain.Projects.Events;

public record ProjectCreatedEvent(UniqueKeyUnit UniqueKey) : DomainEvent, INotification;
