namespace Logitar.Master.Contracts.Accounts;

public record ResetPasswordPayload
{
  public string Locale { get; set; }

  public string? EmailAddress { get; set; }

  public string? Token { get; set; }
  public string? NewPassword { get; set; }

  public ResetPasswordPayload() : this(string.Empty)
  {
  }

  public ResetPasswordPayload(string locale)
  {
    Locale = locale;
  }

  public ResetPasswordPayload(string locale, string emailAddress) : this(locale)
  {
    EmailAddress = emailAddress;
  }

  public ResetPasswordPayload(string locale, string token, string newPassword) : this(locale)
  {
    Token = token;
    NewPassword = newPassword;
  }
}
