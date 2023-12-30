using FluentValidation;
using Logitar.Master.Application.Projects.Validators;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IProjectQuerier _projectQuerier;
  private readonly IProjectRepository _projectRepository;

  public CreateProjectCommandHandler(IApplicationContext applicationContext, IProjectQuerier projectQuerier, IProjectRepository projectRepository)
  {
    _applicationContext = applicationContext;
    _projectQuerier = projectQuerier;
    _projectRepository = projectRepository;
  }

  public async Task<Project> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
  {
    CreateProjectPayload payload = command.Payload;
    new CreateProjectPayloadValidator().ValidateAndThrow(payload);

    UniqueKeyUnit uniqueKey = new(payload.UniqueKey);
    if (await _projectRepository.LoadAsync(uniqueKey, cancellationToken) != null)
    {
      throw new UniqueKeyAlreadyUsedException(uniqueKey, nameof(payload.UniqueKey));
    }

    ProjectAggregate project = new(uniqueKey, _applicationContext.ActorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    project.Update(_applicationContext.ActorId);

    await _projectRepository.SaveAsync(project, cancellationToken);

    return await _projectQuerier.ReadAsync(project, cancellationToken);
  }
}
