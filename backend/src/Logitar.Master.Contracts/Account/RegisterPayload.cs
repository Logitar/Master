namespace Logitar.Master.Contracts.Account;

public record RegisterPayload
{
  public string UniqueName { get; set; } = string.Empty;
  // TODO(fpion): Password

  // TODO(fpion): EmailAddress

  // TODO(fpion): FirstName
  // TODO(fpion): LastName
}
