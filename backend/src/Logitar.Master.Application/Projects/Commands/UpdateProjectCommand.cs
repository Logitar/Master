using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal record UpdateProjectCommand(string Id, UpdateProjectPayload Payload) : IRequest<Project?>;
