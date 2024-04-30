using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public static class OneTimePasswordExtensions
{
  private const string PhoneCountryCodeKey = "PhoneCountryCode";
  private const string PhoneNumberKey = "PhoneNumber";
  private const string PhoneE164FormattedKey = "PhoneE164Formatted";
  private const string PurposeKey = "Purpose";
  private const string UserIdKey = "UserId";

  public static Guid GetUserId(this OneTimePassword oneTimePassword)
  {
    Guid? userId = oneTimePassword.TryGetUserId();
    return userId ?? throw new ArgumentException($"The One-Time Password (OTP) has no '{UserIdKey}' custom attribute.", nameof(oneTimePassword));
  }
  public static Guid? TryGetUserId(this OneTimePassword oneTimePassword)
  {
    CustomAttribute? customAttribute = oneTimePassword.CustomAttributes.SingleOrDefault(x => x.Key == UserIdKey);
    return customAttribute == null ? null : Guid.Parse(customAttribute.Value);
  }
  public static void SetUserId(this CreateOneTimePasswordPayload payload, User user)
  {
    payload.CustomAttributes.Add(new CustomAttribute(UserIdKey, user.Id.ToString()));
  }

  public static Phone GetPhone(this OneTimePassword oneTimePassword)
  {
    return oneTimePassword.TryGetPhone()
      ?? throw new ArgumentException("The One-Time Password (OTP) does not have phone custom attributes.", nameof(oneTimePassword));
  }
  public static Phone? TryGetPhone(this OneTimePassword oneTimePassword)
  {
    Phone phone = new();
    foreach (CustomAttribute customAttribute in oneTimePassword.CustomAttributes)
    {
      switch (customAttribute.Key)
      {
        case PhoneCountryCodeKey:
          phone.CountryCode = customAttribute.Value;
          break;
        case PhoneNumberKey:
          phone.Number = customAttribute.Value;
          break;
        case PhoneE164FormattedKey:
          phone.E164Formatted = customAttribute.Value;
          break;
      }
    }

    if (string.IsNullOrWhiteSpace(phone.Number) || string.IsNullOrWhiteSpace(phone.E164Formatted))
    {
      return null;
    }
    return phone;
  }
  public static void SetPhone(this CreateOneTimePasswordPayload payload, Phone phone)
  {
    if (phone.CountryCode != null)
    {
      payload.CustomAttributes.Add(new CustomAttribute(PhoneCountryCodeKey, phone.CountryCode));
    }
    payload.CustomAttributes.Add(new CustomAttribute(PhoneNumberKey, phone.Number));
    payload.CustomAttributes.Add(new CustomAttribute(PhoneE164FormattedKey, phone.E164Formatted));
  }

  public static void EnsurePurpose(this OneTimePassword oneTimePassword, string purpose)
  {
    if (!oneTimePassword.HasPurpose(purpose))
    {
      throw new InvalidOneTimePasswordPurposeException(oneTimePassword, purpose);
    }
  }
  public static bool HasPurpose(this OneTimePassword oneTimePassword, string purpose)
  {
    return oneTimePassword.TryGetPurpose()?.Equals(purpose, StringComparison.OrdinalIgnoreCase) == true;
  }
  public static string GetPurpose(this OneTimePassword oneTimePassword)
  {
    string? purpose = oneTimePassword.TryGetPurpose();
    return purpose ?? throw new ArgumentException($"The One-Time Password (OTP) has no '{PurposeKey}' custom attribute.", nameof(oneTimePassword));
  }
  public static string? TryGetPurpose(this OneTimePassword oneTimePassword)
  {
    CustomAttribute? customAttribute = oneTimePassword.CustomAttributes.SingleOrDefault(x => x.Key == PurposeKey);
    return customAttribute?.Value;
  }
  public static void SetPurpose(this CreateOneTimePasswordPayload payload, string purpose)
  {
    payload.CustomAttributes.Add(new CustomAttribute(PurposeKey, purpose));
  }
}
