using Logitar.Master.Contracts.Projects;

namespace Logitar.Master.Models.Projects;

public record SearchProjectsParameters : SearchParameters
{
  public SearchProjectsPayload ToPayload()
  {
    SearchProjectsPayload payload = new();
    Fill(payload);

    if (Sort != null)
    {
      payload.Sort = new List<ProjectSortOption>(capacity: Sort.Count);
      foreach (string sort in Sort)
      {
        if (!string.IsNullOrWhiteSpace(sort))
        {
          string trimmed = sort.Trim();
          int index = trimmed.IndexOf(SortSeparator);
          if (index < 0)
          {
            payload.Sort.Add(new ProjectSortOption(Enum.Parse<ProjectSort>(trimmed)));
          }
          else
          {
            bool isDescending = trimmed[..index].Trim().Equals(DescendingKeyword, StringComparison.InvariantCultureIgnoreCase);
            payload.Sort.Add(new ProjectSortOption(Enum.Parse<ProjectSort>(trimmed[(index + 1)..]), isDescending));
          }
        }
      }
    }

    return payload;
  }
}
