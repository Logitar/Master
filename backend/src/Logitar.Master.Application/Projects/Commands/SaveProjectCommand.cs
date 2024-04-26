using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal record SaveProjectCommand(ProjectAggregate Project) : IRequest;
