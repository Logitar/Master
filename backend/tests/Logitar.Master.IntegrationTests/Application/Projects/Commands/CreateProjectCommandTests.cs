using FluentValidation.Results;
using Logitar.EventSourcing;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using Logitar.Master.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application.Projects.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class CreateProjectCommandTests : IntegrationTests
{
  private readonly IProjectRepository _projectRepository;

  public CreateProjectCommandTests() : base()
  {
    _projectRepository = ServiceProvider.GetRequiredService<IProjectRepository>();
  }

  [Fact(DisplayName = "It should create a new project.")]
  public async Task It_should_create_a_new_project()
  {
    CreateProjectPayload payload = new("MASTER")
    {
      DisplayName = "  Master  ",
      Description = "    "
    };
    CreateProjectCommand command = new(payload);
    Project project = await Pipeline.ExecuteAsync(command);

    Assert.Equal(2, project.Version);
    Assert.Equal(Actor, project.CreatedBy);
    Assert.Equal(Actor, project.UpdatedBy);
    Assert.True(project.CreatedOn < project.UpdatedOn);

    Assert.Equal(payload.UniqueKey.Trim(), project.UniqueKey);
    Assert.Equal(payload.DisplayName.Trim(), project.DisplayName);
    Assert.Equal(payload.Description?.CleanTrim(), project.Description);

    ProjectEntity? entity = await MasterContext.Projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == new AggregateId(project.Id).Value);
    Assert.NotNull(entity);
  }

  [Fact(DisplayName = "It should throw UniqueKeyAlreadyUsedException when the unique key is already used.")]
  public async Task It_should_throw_UniqueKeyAlreadyUsedException_when_the_unique_key_is_already_used()
  {
    ProjectAggregate project = new(new UniqueKeyUnit("MASTER"));
    await _projectRepository.SaveAsync(project);

    CreateProjectPayload payload = new(project.UniqueKey.Value);
    CreateProjectCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<UniqueKeyAlreadyUsedException>(async () => await Pipeline.ExecuteAsync(command));
    Assert.Equal(payload.UniqueKey, exception.UniqueKey.Value);
    Assert.Equal("UniqueKey", exception.PropertyName);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    CreateProjectPayload payload = new("MASTER123!");
    CreateProjectCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("AllowedCharactersValidator", error.ErrorCode);
    Assert.Equal("UniqueKey", error.PropertyName);
  }
}
