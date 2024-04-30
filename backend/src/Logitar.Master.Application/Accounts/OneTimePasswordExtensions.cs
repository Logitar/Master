using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public static class OneTimePasswordExtensions
{
  private const string PhoneCountryCodeKey = "PhoneCountryCode";
  private const string PhoneE164FormattedKey = "PhoneE164Formatted";
  private const string PhoneNumberKey = "PhoneNumber";
  private const string PurposeKey = "Purpose";
  private const string UserIdKey = "UserId";

  public static Guid GetUserId(this OneTimePassword oneTimePassword)
  {
    CustomAttribute customAttribute = oneTimePassword.CustomAttributes.SingleOrDefault(x => x.Key == UserIdKey)
      ?? throw new InvalidOperationException($"The One-Time Password (OTP) has no '{UserIdKey}' custom attribute.");
    return Guid.Parse(customAttribute.Value);
  }
  public static void SetUserId(this CreateOneTimePasswordPayload payload, User user) => payload.SetCustomAttribute(UserIdKey, user.Id.ToString());

  public static Phone GetPhone(this OneTimePassword oneTimePassword)
  {
    Phone? phone = oneTimePassword.TryGetPhone();
    return phone ?? throw new InvalidOperationException($"The One-Time Password (OTP) has no phone custom attributes.");
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
        case PhoneE164FormattedKey:
          phone.E164Formatted = customAttribute.Value;
          break;
        case PhoneNumberKey:
          phone.Number = customAttribute.Value;
          break;
      }
    }
    return (phone.Number == null || phone.E164Formatted == null) ? null : phone;
  }
  public static void SetPhone(this CreateOneTimePasswordPayload payload, Phone phone) // TODO(fpion): unit tests
  {
    if (phone.CountryCode != null)
    {
      payload.SetCustomAttribute(PhoneCountryCodeKey, phone.CountryCode);
    }
    payload.SetCustomAttribute(PhoneNumberKey, phone.Number);
    payload.SetCustomAttribute(PhoneE164FormattedKey, phone.E164Formatted);
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
    return purpose ?? throw new InvalidOperationException($"The One-Time Password (OTP) has no '{PurposeKey}' custom attribute.");
  }
  public static string? TryGetPurpose(this OneTimePassword oneTimePassword)
  {
    CustomAttribute? customAttribute = oneTimePassword.CustomAttributes.SingleOrDefault(x => x.Key == PurposeKey);
    return customAttribute?.Value;
  }
  public static void SetPurpose(this CreateOneTimePasswordPayload payload, string purpose) => payload.SetCustomAttribute(PurposeKey, purpose);

  private static void SetCustomAttribute(this CreateOneTimePasswordPayload payload, string key, string value)
  {
    CustomAttribute? customAttribute = payload.CustomAttributes.SingleOrDefault(x => x.Key == key);
    if (customAttribute == null)
    {
      customAttribute = new(key, value);
      payload.CustomAttributes.Add(customAttribute);
    }
    else
    {
      customAttribute.Value = value;
    }
  }
}
