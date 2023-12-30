using MediatR;

namespace Logitar.Master.Infrastructure.Commands;

/// <summary>
/// The command raised to initialize the database.
/// </summary>
public record InitializeDatabaseCommand : INotification;
