namespace Logitar.Master.Contracts.Projects;

/// <summary>
/// Represents a project in the system. A project can be seen as a collection of team members who collaborate on shared tasks.
/// </summary>
public class Project : Aggregate
{
  /// <summary>
  /// Gets or sets the identifier of the project.
  /// </summary>
  public string Id { get; set; } = string.Empty;

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
