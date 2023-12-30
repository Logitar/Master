using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;

namespace Logitar.Master.Application.Projects;

/// <summary>
/// Defines methods to read projects from a data source.
/// </summary>
public interface IProjectQuerier
{
  /// <summary>
  /// Reads the specified project.
  /// </summary>
  /// <param name="project">The project aggregate.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The read project.</returns>
  Task<Project> ReadAsync(ProjectAggregate project, CancellationToken cancellationToken = default);
}
