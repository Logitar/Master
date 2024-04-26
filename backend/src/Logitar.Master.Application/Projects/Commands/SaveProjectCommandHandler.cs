using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class SaveProjectCommandHandler : IRequestHandler<SaveProjectCommand>
{
  private readonly IProjectRepository _projectRepository;

  public SaveProjectCommandHandler(IProjectRepository projectRepository)
  {
    _projectRepository = projectRepository;
  }

  public async Task Handle(SaveProjectCommand command, CancellationToken cancellationToken)
  {
    ProjectAggregate project = command.Project;

    if (await _projectRepository.LoadAsync(project.UniqueKey, cancellationToken) != null)
    {
      throw new UniqueKeyAlreadyUsedException(project.UniqueKey);
    }

    await _projectRepository.SaveAsync(project, cancellationToken);
  }
}
