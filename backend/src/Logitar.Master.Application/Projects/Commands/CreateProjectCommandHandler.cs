using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Application.Projects.Validators;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Commands;

internal class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Project>
{
  private readonly IProjectQuerier _projectQuerier;
  private readonly ISender _sender;

  public CreateProjectCommandHandler(IProjectQuerier projectQuerier, ISender sender)
  {
    _projectQuerier = projectQuerier;
    _sender = sender;
  }

  public async Task<Project> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
  {
    CreateProjectPayload payload = command.Payload;
    new CreateProjectValidator().ValidateAndThrow(payload);

    UniqueKeyUnit uniqueKey = new(payload.UniqueKey);
    ProjectAggregate project = new(uniqueKey, command.ActorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    project.Update(command.ActorId);

    await _sender.Send(new SaveProjectCommand(project), cancellationToken);

    return await _projectQuerier.ReadAsync(project, cancellationToken);
  }
}
