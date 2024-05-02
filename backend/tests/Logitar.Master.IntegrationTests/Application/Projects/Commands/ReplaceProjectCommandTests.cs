using FluentValidation.Results;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using Logitar.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application.Projects.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ReplaceProjectCommandTests : IntegrationTests
{
  private readonly IProjectRepository _projectRepository;

  private readonly ProjectAggregate _project;

  public ReplaceProjectCommandTests() : base()
  {
    _projectRepository = ServiceProvider.GetRequiredService<IProjectRepository>();

    _project = new(new UniqueKeyUnit("MASTER"), ActorId);
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _projectRepository.SaveAsync(_project);
  }

  [Fact(DisplayName = "It should replace an existing project with delta.")]
  public async Task It_should_replace_an_existing_project_with_delta()
  {
    long version = _project.Version;
    _project.Description = new DescriptionUnit("This is the master project.");
    _project.Update(ActorId);
    await _projectRepository.SaveAsync(_project);

    ReplaceProjectPayload payload = new()
    {
      DisplayName = " Master ",
      Description = "        "
    };
    ReplaceProjectCommand command = new(_project.Id.ToGuid(), payload, version);
    Project? project = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(project);

    Assert.Equal(_project.Id.ToGuid(), project.Id);
    Assert.Equal(3, project.Version);
    Assert.Equal(Actor, project.CreatedBy);
    Assert.Equal(Actor, project.UpdatedBy);
    Assert.True(project.CreatedOn < project.UpdatedOn);

    Assert.Equal(_project.UniqueKey.Value, project.UniqueKey);
    Assert.Equal(payload.DisplayName.Trim(), project.DisplayName);
    Assert.Equal(_project.Description.Value, project.Description);
  }

  [Fact(DisplayName = "It should replace an existing project.")]
  public async Task It_should_replace_an_existing_project()
  {
    ReplaceProjectPayload payload = new()
    {
      DisplayName = " Master ",
      Description = "        "
    };
    ReplaceProjectCommand command = new(_project.Id.ToGuid(), payload, Version: null);
    Project? project = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(project);

    Assert.Equal(_project.Id.ToGuid(), project.Id);
    Assert.Equal(2, project.Version);
    Assert.Equal(Actor, project.CreatedBy);
    Assert.Equal(Actor, project.UpdatedBy);
    Assert.True(project.CreatedOn < project.UpdatedOn);

    Assert.Equal(_project.UniqueKey.Value, project.UniqueKey);
    Assert.Equal(payload.DisplayName.Trim(), project.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), project.Description);
  }

  [Fact(DisplayName = "It should return null when the project cannot be found.")]
  public async Task It_should_return_null_when_the_project_cannot_be_found()
  {
    ReplaceProjectPayload payload = new();
    ReplaceProjectCommand command = new(Id: Guid.Empty, payload, Version: null);
    Project? project = await Pipeline.ExecuteAsync(command);
    Assert.Null(project);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ReplaceProjectPayload payload = new()
    {
      DisplayName = RandomStringGenerator.GetString(DisplayNameUnit.MaximumLength + 1)
    };
    ReplaceProjectCommand command = new(_project.Id.ToGuid(), payload, Version: null);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("MaximumLengthValidator", error.ErrorCode);
    Assert.Equal("DisplayName", error.PropertyName);
  }
}
