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
    Dictionary<Guid, Project> projects = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Project? project = await _projectQuerier.ReadAsync(query.Id.Value, cancellationToken);
      if (project != null)
      {
        projects[project.Id] = project;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.UniqueKey))
    {
      Project? project = await _projectQuerier.ReadAsync(query.UniqueKey, cancellationToken);
      if (project != null)
      {
        projects[project.Id] = project;
      }
    }

    if (projects.Count > 1)
    {
      throw TooManyResultsException<Project>.ExpectedSingle(projects.Count);
    }

    return projects.Values.SingleOrDefault();
  }
}
