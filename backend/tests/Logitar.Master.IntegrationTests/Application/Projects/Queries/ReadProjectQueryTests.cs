using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application.Projects.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class ReadProjectQueryTests : IntegrationTests
{
  private readonly IProjectRepository _projectRepository;

  private readonly ProjectAggregate _master;
  private readonly ProjectAggregate _portal;

  public ReadProjectQueryTests() : base()
  {
    _projectRepository = ServiceProvider.GetRequiredService<IProjectRepository>();

    _master = new(new UniqueKeyUnit("MASTER"));
    _portal = new(new UniqueKeyUnit("PORTAL"));
  }

  public override async Task InitializeAsync()
  {
    await base.InitializeAsync();

    await _projectRepository.SaveAsync([_master, _portal]);
  }

  [Fact(DisplayName = "It should return null when no project was found.")]
  public async Task It_should_return_null_when_no_project_was_found()
  {
    ReadProjectQuery query = new(Id: Guid.Empty, UniqueKey: "test");
    Project? project = await Mediator.Send(query);
    Assert.Null(project);
  }

  [Fact(DisplayName = "It should return the project found by ID.")]
  public async Task It_should_return_the_project_found_by_Id()
  {
    ReadProjectQuery query = new(Id: _master.Id.ToGuid(), UniqueKey: null);
    Project? project = await Mediator.Send(query);
    Assert.NotNull(project);
    Assert.Equal(_master.Id.ToGuid(), project.Id);
  }

  [Fact(DisplayName = "It should return the project found by unique key.")]
  public async Task It_should_return_the_project_found_by_unique_key()
  {
    ReadProjectQuery query = new(Id: null, UniqueKey: "  portal  ");
    Project? project = await Mediator.Send(query);
    Assert.NotNull(project);
    Assert.Equal(_portal.Id.ToGuid(), project.Id);
  }

  [Fact(DisplayName = "It should throw TooManyResultsException when many projects were found.")]
  public async Task It_should_throw_TooManyResultsException_when_many_projects_were_found()
  {
    ReadProjectQuery query = new(_master.Id.ToGuid(), UniqueKey: "  portal  ");
    var exception = await Assert.ThrowsAsync<TooManyResultsException<Project>>(async () => await Mediator.Send(query));
    Assert.Equal(1, exception.ExpectedCount);
    Assert.Equal(2, exception.ActualCount);
  }
}
