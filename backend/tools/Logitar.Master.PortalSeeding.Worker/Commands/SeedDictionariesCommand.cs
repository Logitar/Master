using MediatR;

namespace Logitar.Master.PortalSeeding.Worker.Commands;

internal record SeedDictionariesCommand : INotification;
