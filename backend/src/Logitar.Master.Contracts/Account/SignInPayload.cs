namespace Logitar.Master.Contracts.Account;

public record SignInPayload
{
  public string? TenantId { get; set; }
  public string UniqueName { get; set; } = string.Empty;
  // TODO(fpion): Password
  // TODO(fpion): IsPersistent
}
