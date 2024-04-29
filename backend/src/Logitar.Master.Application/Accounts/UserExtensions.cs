using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using System.Globalization;

namespace Logitar.Master.Application.Accounts;

internal static class UserExtensions
{
  private const string MultiFactorAuthenticationModeKey = nameof(MultiFactorAuthenticationMode);
  private const string ProfileCompletedOnKey = "ProfileCompletedOn";

  public static MultiFactorAuthenticationMode? GetMultiFactorAuthenticationMode(this User user)
  {
    CustomAttribute? customAttribute = user.CustomAttributes.SingleOrDefault(x => x.Key == MultiFactorAuthenticationModeKey);
    return customAttribute == null ? null : Enum.Parse<MultiFactorAuthenticationMode>(customAttribute.Value);
  }
  public static void SetMultiFactorAuthenticationMode(this UpdateUserPayload payload, MultiFactorAuthenticationMode mode)
  {
    payload.CustomAttributes.Add(new CustomAttributeModification(MultiFactorAuthenticationModeKey, mode.ToString()));
  }

  public static string GetSubject(this User user) => user.Id.ToString();

  public static void CompleteProfile(this UpdateUserPayload payload)
  {
    string completedOn = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
    payload.CustomAttributes.Add(new CustomAttributeModification(ProfileCompletedOnKey, completedOn));
  }
  public static bool IsProfileCompleted(this User user)
  {
    return user.GetProfileCompleted().HasValue;
  }
  public static DateTime? GetProfileCompleted(this User user)
  {
    CustomAttribute? customAttribute = user.CustomAttributes.SingleOrDefault(x => x.Key == ProfileCompletedOnKey);
    return customAttribute == null ? null : DateTime.Parse(customAttribute.Value);
  }
}
