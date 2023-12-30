namespace Logitar.Master.Contracts.Projects;

/// <summary>
/// The project update payload.
/// </summary>
public record UpdateProjectPayload
{
  /// <summary>
  /// Gets or sets the unique key of the project.
  /// </summary>
  public string? UniqueKey { get; set; }
  /// <summary>
  /// Gets or sets the display name of the project.
  /// </summary>
  public Modification<string>? DisplayName { get; set; }
  /// <summary>
  /// Gets or sets the description of the project.
  /// </summary>
  public Modification<string>? Description { get; set; }
}
