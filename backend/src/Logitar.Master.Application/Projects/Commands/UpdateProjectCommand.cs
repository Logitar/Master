using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

public record UpdateProjectCommand(Guid Id, UpdateProjectPayload Payload) : Activity, IRequest<Project>;
