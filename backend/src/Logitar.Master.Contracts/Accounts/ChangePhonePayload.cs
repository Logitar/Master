namespace Logitar.Master.Contracts.Accounts;

public record ChangePhonePayload
{
  public string Locale { get; set; }

  public AccountPhone? NewPhone { get; set; }
  public OneTimePasswordPayload? OneTimePassword { get; set; }

  public ChangePhonePayload() : this(string.Empty)
  {
  }

  public ChangePhonePayload(string locale)
  {
    Locale = locale;
  }

  public ChangePhonePayload(string locale, AccountPhone newPhone) : this(locale)
  {
    NewPhone = newPhone;
  }

  public ChangePhonePayload(string locale, OneTimePasswordPayload oneTimePassword) : this(locale)
  {
    OneTimePassword = oneTimePassword;
  }
}
