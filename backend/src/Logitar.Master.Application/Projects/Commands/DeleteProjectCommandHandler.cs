using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand, Project?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IProjectQuerier _projectQuerier;
  private readonly IProjectRepository _projectRepository;

  public DeleteProjectCommandHandler(IApplicationContext applicationContext, IProjectQuerier projectQuerier, IProjectRepository projectRepository)
  {
    _applicationContext = applicationContext;
    _projectQuerier = projectQuerier;
    _projectRepository = projectRepository;
  }

  public async Task<Project?> Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
  {
    ProjectId id = ProjectId.Parse(command.Id);
    ProjectAggregate? project = await _projectRepository.LoadAsync(id, cancellationToken);
    if (project == null)
    {
      return null;
    }
    Project result = await _projectQuerier.ReadAsync(project, cancellationToken);

    project.Delete(_applicationContext.ActorId);

    await _projectRepository.SaveAsync(project, cancellationToken);

    return result;
  }
}
