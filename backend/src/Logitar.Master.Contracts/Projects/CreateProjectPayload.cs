namespace Logitar.Master.Contracts.Projects;

public record CreateProjectPayload
{
  public string UniqueKey { get; set; }
  public string? DisplayName { get; set; }
  public string? Description { get; set; }

  public CreateProjectPayload() : this(string.Empty)
  {
  }

  public CreateProjectPayload(string uniqueKey)
  {
    UniqueKey = uniqueKey;
  }
}
