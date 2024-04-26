using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

public record DeleteProjectCommand(Guid Id) : Activity, IRequest<Project?>;
