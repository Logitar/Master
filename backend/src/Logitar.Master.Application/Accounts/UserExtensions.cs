using Logitar.Identity.Contracts;
using Logitar.Identity.Contracts.Users;
using Logitar.Identity.Domain.Users;
using Logitar.Master.Application.Constants;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;

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

  public static Phone? GetPhone(this ValidatedToken validatedToken)
  {
    Phone phone = new();
    foreach (TokenClaim claim in validatedToken.Claims)
    {
      switch (claim.Name)
      {
        case ClaimNames.PhoneCountryCode:
          phone.CountryCode = claim.Value;
          break;
        case ClaimNames.PhoneNumberRaw:
          phone.Number = claim.Value;
          break;
        case Rfc7519ClaimNames.IsPhoneVerified:
          phone.IsVerified = bool.Parse(claim.Value);
          break;
        case Rfc7519ClaimNames.PhoneNumber:
          int index = claim.Value.IndexOf(';');
          if (index < 0)
          {
            phone.E164Formatted = claim.Value;
          }
          else
          {
            phone.E164Formatted = claim.Value[..index];
            phone.Extension = claim.Value[(index + 1)..].Remove("ext=");
          }
          break;
      }
    }
    if (string.IsNullOrWhiteSpace(phone.Number) || string.IsNullOrWhiteSpace(phone.E164Formatted))
    {
      return null;
    }
    return phone;
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

  public static EmailPayload ToEmailPayload(this Email email) => email.ToEmailPayload(email.IsVerified);
  public static EmailPayload ToEmailPayload(this IEmail email, bool isVerified = false) => new(email.Address, isVerified);

  public static Phone ToPhone(this AccountPhone phone)
  {
    Phone result = new(phone.CountryCode?.CleanTrim(), phone.Number.Trim(), extension: null, e164Formatted: string.Empty);
    result.E164Formatted = result.FormatToE164();
    return result;
  }

  public static PhonePayload ToPhonePayload(this Phone phone) => phone.ToPhonePayload(phone.IsVerified);
  public static PhonePayload ToPhonePayload(this IPhone phone, bool isVerified = false) => new(phone.CountryCode, phone.Number, phone.Extension, isVerified);

  public static UpdateUserPayload ToUpdateUserPayload(this SaveProfilePayload payload) => new()
  {
    FirstName = new Modification<string>(payload.FirstName),
    MiddleName = new Modification<string>(payload.MiddleName),
    LastName = new Modification<string>(payload.LastName),
    Birthdate = new Modification<DateTime?>(payload.Birthdate),
    Gender = new Modification<string>(payload.Gender),
    Locale = new Modification<string>(payload.Locale),
    TimeZone = new Modification<string>(payload.TimeZone)
  };

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
