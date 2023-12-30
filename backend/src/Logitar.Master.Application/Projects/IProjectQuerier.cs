using Logitar.Master.Contracts.Projects;
using Logitar.Master.Contracts.Search;
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
  /// <summary>
  /// Reads the specified project.
  /// </summary>
  /// <param name="id">The project identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The read project.</returns>
  Task<Project?> ReadAsync(string id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Reads the specified project.
  /// </summary>
  /// <param name="uniqueKey">The project unique key.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The read project.</returns>
  Task<Project?> ReadByUniqueKeyAsync(string uniqueKey, CancellationToken cancellationToken = default);
  /// <summary>
  /// Searches the specified projects.
  /// </summary>
  /// <param name="payload">The search payload.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The search results.</returns>
  Task<SearchResults<Project>> SearchAsync(SearchProjectsPayload payload, CancellationToken cancellationToken = default);
}
