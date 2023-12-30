namespace Logitar.Master.Contracts.Projects;

/// <summary>
/// Defines methods to manage projects.
/// </summary>
public interface IProjectService
{
  /// <summary>
  /// Creates a new project.
  /// </summary>
  /// <param name="payload">The creation payload.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The created project.</returns>
  Task<Project?> CreateAsync(CreateProjectPayload payload, CancellationToken cancellationToken = default);
  /// <summary>
  /// Deletes the specified project.
  /// </summary>
  /// <param name="id">The identifier of the project.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The deleted project.</returns>
  Task<Project?> DeleteAsync(string id, CancellationToken cancellationToken = default);
  /// <summary>
  /// Replaces the specified project.
  /// </summary>
  /// <param name="id">The identifier of the project.</param>
  /// <param name="payload">The replacement payload.</param>
  /// <param name="version">The reference version to compare changes.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated project.</returns>
  Task<Project?> ReplaceAsync(string id, ReplaceProjectPayload payload, long? version = null, CancellationToken cancellationToken = default);
  /// <summary>
  /// Updates the specified project.
  /// </summary>
  /// <param name="id">The identifier of the project.</param>
  /// <param name="payload">The update payload.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  /// <returns>The updated project.</returns>
  Task<Project?> UpdateAsync(string id, UpdateProjectPayload payload, CancellationToken cancellationToken = default);
}
