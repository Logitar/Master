using Logitar.Master.Contracts.Search;

namespace Logitar.Master.Contracts.Projects;

/// <summary>
/// Represents a project sorting option.
/// </summary>
public record ProjectSortOption : SortOption
{
  /// <summary>
  /// Gets or sets the sort field.
  /// </summary>
  public new ProjectSort Field => Enum.Parse<ProjectSort>(base.Field);

  /// <summary>
  /// Initializes a new instance of the <see cref="ProjectSortOption"/> class.
  /// </summary>
  public ProjectSortOption() : base()
  {
  }
  /// <summary>
  /// Initializes a new instance of the <see cref="ProjectSortOption"/> class.
  /// </summary>
  /// <param name="field">The sort field.</param>
  /// <param name="isDescending">A value indicating whether or not the sort is descending.</param>
  public ProjectSortOption(ProjectSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
