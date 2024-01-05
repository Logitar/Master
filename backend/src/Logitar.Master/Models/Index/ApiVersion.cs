namespace Logitar.Master.Models.Index;

public record ApiVersion
{
  public string Title { get; }
  public string Version { get; }

  public ApiVersion(string title, Version version)
  {
    Title = title;
    Version = version.ToString();
  }
}
