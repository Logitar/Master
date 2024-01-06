namespace Logitar.Master.Contracts.Sessions;

public record RenewPayload
{
  public string RefreshToken { get; set; } = string.Empty;
}
