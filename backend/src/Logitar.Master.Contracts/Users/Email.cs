namespace Logitar.Master.Contracts.Users;

public record Email : Contact
{
  public string Address { get; set; } = string.Empty;
}
