using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
using Logitar.Master.Domain.Projects;
using Microsoft.Extensions.DependencyInjection;

namespace Logitar.Master.Application.Projects.Queries;

[Trait(Traits.Category, Categories.Integration)]
public class SearchProjectQueryTests : IntegrationTests
{
  private readonly IProjectRepository _projectRepository;

  public SearchProjectQueryTests() : base()
  {
    _projectRepository = ServiceProvider.GetRequiredService<IProjectRepository>();
  }

  [Fact(DisplayName = "It should return empty results when none match.")]
  public async Task It_should_return_empty_results_when_none_match()
  {
    SearchProjectsPayload payload = new();
    SearchProjectsQuery query = new(payload);
    SearchResults<Project> results = await Mediator.Send(query);

    Assert.Empty(results.Items);
    Assert.Equal(0, results.Total);
  }

  [Fact(DisplayName = "It should return the correct search results.")]
  public async Task It_should_return_the_correct_search_results()
  {
    ProjectAggregate oubliette = new(new UniqueKeyUnit("OUBLIETTE"));
    ProjectAggregate skillCraft = new(new UniqueKeyUnit("SKILLCRAFT"));
    ProjectAggregate portal = new(new UniqueKeyUnit("PORTAL"));
    ProjectAggregate master = new(new UniqueKeyUnit("MASTER"));
    await _projectRepository.SaveAsync([oubliette, skillCraft, portal, master]);

    List<Guid> ids = (await _projectRepository.LoadAsync()).Select(project => project.Id.ToGuid()).ToList();
    ids.Remove(oubliette.Id.ToGuid());
    ids.Add(Guid.Empty);

    SearchProjectsPayload payload = new()
    {
      Ids = ids,
      Search = new TextSearch([new SearchTerm("%o%"), new SearchTerm("%M%")], SearchOperator.Or),
      Sort = [new ProjectSortOption(ProjectSort.UniqueKey, isDescending: true)],
      Skip = 1,
      Limit = 1
    };
    SearchProjectsQuery query = new(payload);

    SearchResults<Project> results = await Mediator.Send(query);
    Assert.Equal(2, results.Total);

    Project project = Assert.Single(results.Items);
    Assert.Equal(master.Id.ToGuid(), project.Id);
  }
}
