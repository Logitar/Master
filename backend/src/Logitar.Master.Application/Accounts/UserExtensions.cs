using Logitar.Identity.Domain.Users;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public static class UserExtensions
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

  public static Phone ToPhone(this AccountPhone phone)
  {
    Phone result = new(phone.CountryCode, phone.Number, extension: null, e164Formatted: string.Empty);
    result.E164Formatted = result.FormatToE164();
    return result;
  }

  public static UserProfile ToUserProfile(this User user) => new()
  {
    CreatedOn = user.CreatedOn,
    CompletedOn = user.GetProfileCompleted() ?? default,
    UpdatedOn = user.UpdatedOn,
    PasswordChangedOn = user.PasswordChangedOn,
    AuthenticatedOn = user.AuthenticatedOn,
    MultiFactorAuthenticationMode = user.GetMultiFactorAuthenticationMode() ?? MultiFactorAuthenticationMode.None,
    EmailAddress = user.Email?.Address ?? user.UniqueName,
    Phone = AccountPhone.TryCreate(user.Phone),
    FirstName = user.FirstName ?? string.Empty,
    MiddleName = user.MiddleName,
    LastName = user.LastName ?? string.Empty,
    FullName = user.FullName ?? string.Empty,
    Birthdate = user.Birthdate,
    Gender = user.Gender,
    Locale = user.Locale ?? new Locale(),
    TimeZone = user.TimeZone ?? string.Empty
  };
}
