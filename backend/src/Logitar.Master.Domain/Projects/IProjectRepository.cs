namespace Logitar.Master.Domain.Projects;

/// <summary>
/// Defines methods to load and save projects from a data source.
/// </summary>
public interface IProjectRepository
{
  /// <summary>
  /// Loads the specified project.
  /// </summary>
  /// <param name="id">The identifier of the project.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded project.</returns>
  Task<ProjectAggregate?> LoadAsync(ProjectId id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Loads the specified project.
  /// </summary>
  /// <param name="id">The identifier of the project.</param>
  /// <param name="version">The version of the project.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded project.</returns>
  Task<ProjectAggregate?> LoadAsync(ProjectId id, long? version, CancellationToken cancellationToken = default);
  /// <summary>
  /// Loads the specified project.
  /// </summary>
  /// <param name="uniqueKey">The unique key of the project.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The loaded project.</returns>
  Task<ProjectAggregate?> LoadAsync(UniqueKeyUnit uniqueKey, CancellationToken cancellationToken = default);

  /// <summary>
  /// Saves the specified project.
  /// </summary>
  /// <param name="project">The project to save.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The asynchronous operation.</returns>
  Task SaveAsync(ProjectAggregate project, CancellationToken cancellationToken = default);
}
