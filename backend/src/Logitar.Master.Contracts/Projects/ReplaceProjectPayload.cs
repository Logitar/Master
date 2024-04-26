namespace Logitar.Master.Contracts.Projects;

public record ReplaceProjectPayload
{
  public string? DisplayName { get; set; }
  public string? Description { get; set; }
}
