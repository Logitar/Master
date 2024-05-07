using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Application.Constants;

internal static class Templates
{
  public const string AccountAuthentication = "AccountAuthentication";
  public const string PasswordRecovery = "PasswordRecovery";

  private const string ContactVerification = "ContactVerification{ContactType}";
  private const string MultiFactorAuthentication = "MultiFactorAuthentication{ContactType}";

  public static string GetContactVerification(ContactType contactType) => contactType switch
  {
    ContactType.Email or ContactType.Phone => ContactVerification.Replace("{ContactType}", contactType.ToString()),
    _ => throw new ArgumentException($"The contact type '{contactType}' is not supported.", nameof(contactType)),
  };
  public static string GetMultiFactorAuthentication(ContactType contactType) => contactType switch
  {
    ContactType.Email or ContactType.Phone => MultiFactorAuthentication.Replace("{ContactType}", contactType.ToString()),
    _ => throw new ArgumentException($"The contact type '{contactType}' is not supported.", nameof(contactType)),
  };
}
