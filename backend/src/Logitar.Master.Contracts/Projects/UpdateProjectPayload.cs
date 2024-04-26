namespace Logitar.Master.Contracts.Projects;

public record UpdateProjectPayload
{
  public Change<string>? DisplayName { get; set; }
  public Change<string>? Description { get; set; }
}
