namespace Logitar.Master.Contracts.Account;

public record RegisterPayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? Password { get; set; }

  public string? EmailAddress { get; set; }

  public string? FirstName { get; set; }
  public string? LastName { get; set; }
}
