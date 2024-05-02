using MediatR;

namespace Logitar.Master.PortalSeeding.Worker.Commands;

internal record SeedTemplatesCommand : INotification;
