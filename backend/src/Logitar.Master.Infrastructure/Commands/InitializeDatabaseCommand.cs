using MediatR;

namespace Logitar.Master.Infrastructure.Commands;

public record InitializeDatabaseCommand : INotification;
