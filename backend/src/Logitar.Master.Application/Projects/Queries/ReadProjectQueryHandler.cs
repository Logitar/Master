using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Queries;

internal class ReadProjectQueryHandler : IRequestHandler<ReadProjectQuery, Project?>
{
  private readonly IProjectQuerier _projectQuerier;

  public ReadProjectQueryHandler(IProjectQuerier projectQuerier)
  {
    _projectQuerier = projectQuerier;
  }

  public async Task<Project?> Handle(ReadProjectQuery query, CancellationToken cancellationToken)
  {
    Dictionary<string, Project> results = new(capacity: 2);

    if (!string.IsNullOrWhiteSpace(query.Id))
    {
      Project? project = await _projectQuerier.ReadAsync(query.Id, cancellationToken);
      if (project != null)
      {
        results[project.Id] = project;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueKey))
    {
      Project? project = await _projectQuerier.ReadByUniqueKeyAsync(query.UniqueKey, cancellationToken);
      if (project != null)
      {
        results[project.Id] = project;
      }
    }

    if (results.Count > 1)
    {
      throw new TooManyResultsException<Project>(expectedCount: 1, results.Count);
    }

    return results.Values.SingleOrDefault();
  }
}
