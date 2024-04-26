using Logitar.Master.Contracts.Search;

namespace Logitar.Master.Contracts.Projects;

public record ProjectSortOption : SortOption
{
  public new ProjectSort Field
  {
    get => Enum.Parse<ProjectSort>(base.Field);
    set => base.Field = value.ToString();
  }
  public ProjectSortOption() : this(ProjectSort.UpdatedOn, isDescending: true)
  {
  }

  public ProjectSortOption(ProjectSort field, bool isDescending = false) : base(field.ToString(), isDescending)
  {
  }
}
