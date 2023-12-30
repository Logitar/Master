namespace Logitar.Master.Contracts.Projects;

/// <summary>
/// The project replacement payload.
/// </summary>
public record ReplaceProjectPayload
{
  /// <summary>
  /// Gets or sets the unique key of the project.
  /// </summary>
  public string UniqueKey { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the display name of the project.
  /// </summary>
  public string? DisplayName { get; set; }
  /// <summary>
  /// Gets or sets the description of the project.
  /// </summary>
  public string? Description { get; set; }
}
