using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal record DeleteProjectCommand(string Id) : IRequest<Project?>;
