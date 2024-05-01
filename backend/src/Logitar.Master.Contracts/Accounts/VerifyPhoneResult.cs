namespace Logitar.Master.Contracts.Accounts;

public record VerifyPhoneResult
{
  public OneTimePasswordValidation? OneTimePasswordValidation { get; set; }
  public string? ProfileCompletionToken { get; set; }

  public VerifyPhoneResult()
  {
  }

  public VerifyPhoneResult(OneTimePasswordValidation oneTimePasswordValidation)
  {
    OneTimePasswordValidation = oneTimePasswordValidation;
  }

  public VerifyPhoneResult(string profileCompletionToken)
  {
    ProfileCompletionToken = profileCompletionToken;
  }
}
