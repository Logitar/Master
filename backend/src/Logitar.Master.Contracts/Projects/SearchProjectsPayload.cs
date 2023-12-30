using Logitar.Master.Contracts.Search;

namespace Logitar.Master.Contracts.Projects;

/// <summary>
/// The project search payload.
/// </summary>
public record SearchProjectsPayload : SearchPayload
{
  /// <summary>
  /// Gets or sets the sort options.
  /// </summary>
  public new List<ProjectSortOption> Sort { get; set; } = [];
}
