using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
using MediatR;

namespace Logitar.Master.Application.Projects.Queries;

internal class SearchProjectsQueryHandler : IRequestHandler<SearchProjectsQuery, SearchResults<Project>>
{
  private readonly IProjectQuerier _projectQuerier;

  public SearchProjectsQueryHandler(IProjectQuerier projectQuerier)
  {
    _projectQuerier = projectQuerier;
  }

  public async Task<SearchResults<Project>> Handle(SearchProjectsQuery query, CancellationToken cancellationToken)
  {
    return await _projectQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
