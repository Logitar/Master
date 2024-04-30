namespace Logitar.Master.Contracts.Accounts;

public record ChangePhoneResult
{
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public UserProfile? UserProfile { get; set; }

  public ChangePhoneResult()
  {
  }

  public ChangePhoneResult(OneTimePasswordValidation oneTimePasswordValidation)
  {
    OneTimePasswordValidation = oneTimePasswordValidation;
  }

  public ChangePhoneResult(UserProfile userProfile)
  {
    UserProfile = userProfile;
  }
}
