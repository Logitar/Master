using MediatR;

namespace Logitar.Master.PortalSeeding.Worker.Commands;

internal record SeedRealmCommand : INotification;
