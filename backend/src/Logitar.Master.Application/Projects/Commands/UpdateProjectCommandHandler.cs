using FluentValidation;
using Logitar.Master.Application.Projects.Validators;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, Project?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IProjectQuerier _projectQuerier;
  private readonly IProjectRepository _projectRepository;

  public UpdateProjectCommandHandler(IApplicationContext applicationContext, IProjectQuerier projectQuerier, IProjectRepository projectRepository)
  {
    _applicationContext = applicationContext;
    _projectQuerier = projectQuerier;
    _projectRepository = projectRepository;
  }

  public async Task<Project?> Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
  {
    UpdateProjectPayload payload = command.Payload;
    new UpdateProjectPayloadValidator().ValidateAndThrow(payload);

    ProjectId id = ProjectId.Parse(command.Id);
    ProjectAggregate? project = await _projectRepository.LoadAsync(id, cancellationToken);
    if (project == null)
    {
      return null;
    }

    UniqueKeyUnit? uniqueKey = UniqueKeyUnit.TryCreate(payload.UniqueKey);
    if (uniqueKey != null)
    {
      project.SetUniqueKey(uniqueKey, _applicationContext.ActorId);
    }
    if (payload.DisplayName != null)
    {
      project.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      project.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    project.Update(_applicationContext.ActorId);

    await _projectRepository.SaveAsync(project, cancellationToken);

    return await _projectQuerier.ReadAsync(project, cancellationToken);
  }
}
