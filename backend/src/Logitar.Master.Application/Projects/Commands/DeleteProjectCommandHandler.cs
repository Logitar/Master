using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Project?>
{
  private readonly IProjectQuerier _projectQuerier;
  private readonly IProjectRepository _projectRepository;
  private readonly ISender _sender;

  public DeleteProjectCommandHandler(IProjectQuerier projectQuerier, IProjectRepository projectRepository, ISender sender)
  {
    _projectQuerier = projectQuerier;
    _projectRepository = projectRepository;
    _sender = sender;
  }

  public async Task<Project?> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
  {
    ProjectAggregate? project = await _projectRepository.LoadAsync(command.Id, cancellationToken);
    if (project == null)
    {
      return null;
    }
    Project result = await _projectQuerier.ReadAsync(project, cancellationToken);

    project.Delete(command.ActorId);

    await _projectRepository.SaveAsync(project, cancellationToken);

    return result;
  }
}
