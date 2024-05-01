namespace Logitar.Master.Contracts.Accounts;

public record VerifyPhonePayload
{
  public string Locale { get; set; }
  public string ProfileCompletionToken { get; set; }

  public AccountPhone? NewPhone { get; set; }
  public OneTimePasswordPayload? OneTimePassword { get; set; }

  public VerifyPhonePayload() : this(string.Empty, string.Empty)
  {
  }

  public VerifyPhonePayload(string locale, string profileCompletionToken)
  {
    Locale = locale;
    ProfileCompletionToken = profileCompletionToken;
  }

  public VerifyPhonePayload(string locale, string profileCompletionToken, AccountPhone newPhone) : this(locale, profileCompletionToken)
  {
    NewPhone = newPhone;
  }

  public VerifyPhonePayload(string locale, string profileCompletionToken, OneTimePasswordPayload oneTimePassword) : this(locale, profileCompletionToken)
  {
    OneTimePassword = oneTimePassword;
  }
}
