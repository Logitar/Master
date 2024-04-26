using Logitar.EventSourcing;
using Logitar.Master.Domain.Projects;
using Logitar.Master.Domain.Projects.Events;
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

    bool hasUniqueKeyChanged = false;
    foreach (DomainEvent change in project.Changes)
    {
      if (change is ProjectCreatedEvent)
      {
        hasUniqueKeyChanged = true;
      }
    }

    if (hasUniqueKeyChanged)
    {
      ProjectAggregate? conflict = await _projectRepository.LoadAsync(project.UniqueKey, cancellationToken);
      if (conflict?.Equals(project) == false)
      {
        throw new UniqueKeyAlreadyUsedException(conflict.UniqueKey);
      }
    }

    await _projectRepository.SaveAsync(project, cancellationToken);
  }
}
