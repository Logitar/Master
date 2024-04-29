using Logitar.Portal.Contracts;

namespace Logitar.Master.Contracts.Projects;

public class Project : Aggregate
{
  public string UniqueKey { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public Project() : this(string.Empty)
  {
  }

  public Project(string uniqueKey)
  {
    UniqueKey = uniqueKey;
  }

  public override string ToString() => $"{DisplayName ?? UniqueKey} | {base.ToString()}";
}
