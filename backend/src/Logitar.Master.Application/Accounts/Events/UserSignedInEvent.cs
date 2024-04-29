using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Accounts.Events;

public record UserSignedInEvent(Session Session) : INotification;
