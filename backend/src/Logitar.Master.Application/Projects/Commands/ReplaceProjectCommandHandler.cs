using FluentValidation;
using Logitar.Master.Application.Projects.Validators;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class ReplaceProjectCommandHandler : IRequestHandler<ReplaceProjectCommand, Project?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IProjectQuerier _projectQuerier;
  private readonly IProjectRepository _projectRepository;

  public ReplaceProjectCommandHandler(IApplicationContext applicationContext, IProjectQuerier projectQuerier, IProjectRepository projectRepository)
  {
    _applicationContext = applicationContext;
    _projectQuerier = projectQuerier;
    _projectRepository = projectRepository;
  }

  public async Task<Project?> Handle(ReplaceProjectCommand command, CancellationToken cancellationToken)
  {
    ReplaceProjectPayload payload = command.Payload;
    new ReplaceProjectPayloadValidator().ValidateAndThrow(payload);

    ProjectId id = ProjectId.Parse(command.Id);
    ProjectAggregate? project = await _projectRepository.LoadAsync(id, cancellationToken);
    if (project == null)
    {
      return null;
    }
    ProjectAggregate? reference = command.Version.HasValue ? await _projectRepository.LoadAsync(id, command.Version.Value, cancellationToken) : null;

    if (reference == null || (payload.UniqueKey.Trim() != reference.UniqueKey.Value))
    {
      project.SetUniqueKey(new UniqueKeyUnit(payload.UniqueKey), _applicationContext.ActorId);
    }
    if (reference == null || (payload.DisplayName?.CleanTrim() != reference.DisplayName?.Value))
    {
      project.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    }
    if (reference == null || (payload.Description?.CleanTrim() != reference.Description?.Value))
    {
      project.Description = DescriptionUnit.TryCreate(payload.Description);
    }

    project.Update(_applicationContext.ActorId);

    await _projectRepository.SaveAsync(project, cancellationToken);

    return await _projectQuerier.ReadAsync(project, cancellationToken);
  }
}
