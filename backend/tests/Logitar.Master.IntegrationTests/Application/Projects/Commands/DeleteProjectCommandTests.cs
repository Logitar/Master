using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using Logitar.Master.EntityFrameworkCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application.Projects.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class DeleteProjectCommandTests : IntegrationTests
{
  private readonly IProjectRepository _projectRepository;

  private readonly ProjectAggregate _project;

  public DeleteProjectCommandTests() : base()
  {
    _projectRepository = ServiceProvider.GetRequiredService<IProjectRepository>();

    _project = new(new UniqueKeyUnit("MASTER"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _projectRepository.SaveAsync(_project);
  }

  [Fact(DisplayName = "It should update an existing project.")]
  public async Task It_should_delete_an_existing_project()
  {
    DeleteProjectCommand command = new(_project.Id.ToGuid());
    Project? project = await Pipeline.ExecuteAsync(command);
    Assert.NotNull(project);
    Assert.Equal(_project.Id.ToGuid(), project.Id);

    ProjectEntity? entity = await MasterContext.Projects.AsNoTracking()
      .SingleOrDefaultAsync(x => x.AggregateId == _project.Id.AggregateId.Value);
    Assert.Null(entity);
  }

  [Fact(DisplayName = "It should return null when the project cannot be found.")]
  public async Task It_should_return_null_when_the_project_cannot_be_found()
  {
    DeleteProjectCommand command = new(Id: Guid.Empty);
    Project? project = await Pipeline.ExecuteAsync(command);
    Assert.Null(project);
  }
}
