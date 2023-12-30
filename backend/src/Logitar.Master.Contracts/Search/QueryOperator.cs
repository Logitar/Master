namespace Logitar.Master.Contracts.Search;

/// <summary>
/// The available query operators.
/// </summary>
public enum QueryOperator
{
  /// <summary>
  /// All conditions must be met for a result to match.
  /// </summary>
  And = 0,

  /// <summary>
  /// At least one condition must be met for a result to match.
  /// </summary>
  Or = 1
}
