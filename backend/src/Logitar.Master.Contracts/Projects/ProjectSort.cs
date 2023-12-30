namespace Logitar.Master.Contracts.Projects;

/// <summary>
/// The available project sort fields.
/// </summary>
public enum ProjectSort
{
  /// <summary>
  /// The projects will be sorted by their display name.
  /// </summary>
  DisplayName,

  /// <summary>
  /// The projects will be sorted by their unique key.
  /// </summary>
  UniqueKey,

  /// <summary>
  /// The projects will be sorted by their latest update date and time.
  /// </summary>
  UpdatedOn
}
