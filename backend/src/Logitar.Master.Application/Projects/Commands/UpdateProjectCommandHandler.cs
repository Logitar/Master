using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Application.Projects.Validators;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Project?>
{
  private readonly IProjectQuerier _projectQuerier;
  private readonly IProjectRepository _projectRepository;
  private readonly ISender _sender;

  public UpdateProjectCommandHandler(IProjectQuerier projectQuerier, IProjectRepository projectRepository, ISender sender)
  {
    _projectQuerier = projectQuerier;
    _projectRepository = projectRepository;
    _sender = sender;
  }

  public async Task<Project?> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
  {
    UpdateProjectPayload payload = command.Payload;
    new UpdateProjectValidator().ValidateAndThrow(payload);

    ProjectAggregate? project = await _projectRepository.LoadAsync(command.Id, cancellationToken);
    if (project == null)
    {
      return null;
    }

    if (payload.DisplayName != null)
    {
      project.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      project.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    project.Update(command.ActorId);

    await _sender.Send(new SaveProjectCommand(project), cancellationToken);

    return await _projectQuerier.ReadAsync(project, cancellationToken);
  }
}
