using Logitar.Portal.Contracts.Tokens;

namespace Logitar.Master.Contracts.Accounts;

public record ChangePhoneResult
{
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public UserProfile? UserProfile { get; set; }
  public string? ProfileCompletionToken { get; set; }

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

  public ChangePhoneResult(CreatedToken profileCompletion) : this(profileCompletion.Token)
  {
  }
  public ChangePhoneResult(string profileCompletionToken)
  {
    ProfileCompletionToken = profileCompletionToken;
  }
}
