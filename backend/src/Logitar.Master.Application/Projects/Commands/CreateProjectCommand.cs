using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

public record CreateProjectCommand(CreateProjectPayload Payload) : Activity, IRequest<Project>;
