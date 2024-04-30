namespace Logitar.Master.Contracts.Accounts;

public record ChangePhonePayload
{
  public string Locale { get; set; }

  public AccountPhone? Phone { get; set; }
  public OneTimePasswordPayload? OneTimePassword { get; set; }
  public string? ProfileCompletionToken { get; set; }

  public ChangePhonePayload() : this(string.Empty)
  {
  }

  public ChangePhonePayload(string locale)
  {
    Locale = locale;
  }

  public ChangePhonePayload(string locale, AccountPhone phone, string? profileCompletionToken = null) : this(locale)
  {
    Phone = phone;
    ProfileCompletionToken = profileCompletionToken;
  }

  public ChangePhonePayload(string locale, OneTimePasswordPayload oneTimePassword, string? profileCompletionToken = null) : this(locale)
  {
    OneTimePassword = oneTimePassword;
    ProfileCompletionToken = profileCompletionToken;
  }
}
