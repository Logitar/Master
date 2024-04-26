using Logitar.Portal.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

public record CreateProjectCommand(CreateProjectPayload Payload) : Activity, IRequest<Project>;
