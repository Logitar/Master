namespace Logitar.Master.Contracts.Accounts;

public record Credentials
{
  public string EmailAddress { get; set; }
  public string? Password { get; set; }

  public Credentials() : this(string.Empty)
  {
  }

  public Credentials(string emailAddress, string? password = null)
  {
    EmailAddress = emailAddress;
    Password = password;
  }
}
