using Logitar.Master.Contracts.Search;

namespace Logitar.Master.Contracts.Projects;

public record SearchProjectsPayload : SearchPayload
{
  public new List<ProjectSortOption>? Sort { get; set; }
}
