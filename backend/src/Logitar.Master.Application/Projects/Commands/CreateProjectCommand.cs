using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal record CreateProjectCommand(CreateProjectPayload Payload) : IRequest<Project>;
