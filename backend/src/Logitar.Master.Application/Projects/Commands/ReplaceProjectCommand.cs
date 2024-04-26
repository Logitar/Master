using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

public record ReplaceProjectCommand(Guid Id, ReplaceProjectPayload Payload, long? Version) : Activity, IRequest<Project?>;
