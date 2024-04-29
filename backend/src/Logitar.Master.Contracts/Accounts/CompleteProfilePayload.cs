namespace Logitar.Master.Contracts.Accounts;

public record CompleteProfilePayload : SaveProfilePayload
{
  public string Token { get; set; }

  public string? Password { get; set; }
  public MultiFactorAuthenticationMode MultiFactorAuthenticationMode { get; set; }

  public string? PhoneNumber { get; set; } // TODO(fpion): CountryCode? IsVerified?

  public CompleteProfilePayload() : this(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty)
  {
  }

  public CompleteProfilePayload(string token, string firstName, string lastName, string locale, string timeZone)
    : base(firstName, lastName, locale, timeZone)
  {
    Token = token;
  }
}
