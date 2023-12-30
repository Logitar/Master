using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal record ReplaceProjectCommand(string Id, ReplaceProjectPayload Payload, long? Version) : IRequest<Project?>;
