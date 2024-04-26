using FluentValidation;
using Logitar.Master.Application.Projects.Validators;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using Logitar.Master.Domain.Shared;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class ReplaceProjectCommandHandler : IRequestHandler<ReplaceProjectCommand, Project?>
{
  private readonly IProjectQuerier _projectQuerier;
  private readonly IProjectRepository _projectRepository;
  private readonly ISender _sender;

  public ReplaceProjectCommandHandler(IProjectQuerier projectQuerier, IProjectRepository projectRepository, ISender sender)
  {
    _projectQuerier = projectQuerier;
    _projectRepository = projectRepository;
    _sender = sender;
  }

  public async Task<Project?> Handle(ReplaceProjectCommand command, CancellationToken cancellationToken)
  {
    ReplaceProjectPayload payload = command.Payload;
    new ReplaceProjectValidator().ValidateAndThrow(payload);

    ProjectAggregate? project = await _projectRepository.LoadAsync(command.Id, cancellationToken);
    if (project == null)
    {
      return null;
    }

    ProjectAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _projectRepository.LoadAsync(project.Id, command.Version.Value, cancellationToken);
    }

    if (reference == null)
    {
      project.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName);
      project.Description = DescriptionUnit.TryCreate(payload.Description);
    }
    else
    {
      DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
      if (displayName != reference.DisplayName)
      {
        project.DisplayName = displayName;
      }
      DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
      if (description != reference.Description)
      {
        project.Description = description;
      }
    }

    project.Update(command.ActorId);

    await _sender.Send(new SaveProjectCommand(project), cancellationToken);

    return await _projectQuerier.ReadAsync(project, cancellationToken);
  }
}
