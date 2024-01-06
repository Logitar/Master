namespace Logitar.Master.Contracts.Account;

public record RegisterPayload
{
  public string UniqueName { get; set; } = string.Empty;
  public string? Password { get; set; }

  // TODO(fpion): EmailAddress

  // TODO(fpion): FirstName
  // TODO(fpion): LastName
}
