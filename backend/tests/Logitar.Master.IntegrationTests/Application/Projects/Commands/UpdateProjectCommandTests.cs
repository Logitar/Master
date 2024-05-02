using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application.Projects.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class UpdateProjectCommandTests : IntegrationTests
{
  private readonly IProjectRepository _projectRepository;

  private readonly ProjectAggregate _project;

  public UpdateProjectCommandTests() : base()
  {
    _projectRepository = ServiceProvider.GetRequiredService<IProjectRepository>();

    _project = new(new UniqueKeyUnit("MASTER"), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _projectRepository.SaveAsync(_project);
  }

  [Fact(DisplayName = "It should return null when the project cannot be found.")]
  public async Task It_should_return_null_when_the_project_cannot_be_found()
  {
    UpdateProjectPayload payload = new();
    UpdateProjectCommand command = new(Id: Guid.Empty, payload);
    Project? project = await Pipeline.ExecuteAsync(command);
    Assert.Null(project);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    UpdateProjectPayload payload = new()
    {
      DisplayName = new Change<string>(RandomStringGenerator.GetString(DisplayNameUnit.MaximumLength + 1))
    };
    UpdateProjectCommand command = new(_project.Id.ToGuid(), payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("DisplayName.Value", error.PropertyName);
  }

  [Fact(DisplayName = "It should update an existing project.")]
  public async Task It_should_update_an_existing_project()
  {
    _project.DisplayName = new DisplayNameUnit("Master");
    _project.Description = new DescriptionUnit("This is the master project.");
    _project.Update(ActorId);
    await _projectRepository.SaveAsync(_project);

    UpdateProjectPayload payload = new()
    {
      Description = new Change<string>("    ")
    };
    UpdateProjectCommand command = new(_project.Id.ToGuid(), payload);
    Project? project = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(project);

    Assert.Equal(_project.Id.ToGuid(), project.Id);
    Assert.Equal(3, project.Version);
    Assert.Equal(Actor, project.CreatedBy);
    Assert.Equal(Actor, project.UpdatedBy);
    Assert.True(project.CreatedOn < project.UpdatedOn);

    Assert.Equal(_project.UniqueKey.Value, project.UniqueKey);
    Assert.Equal(_project.DisplayName.Value, project.DisplayName);
    Assert.Equal(payload.Description.Value?.CleanTrim(), project.Description);
  }
}
