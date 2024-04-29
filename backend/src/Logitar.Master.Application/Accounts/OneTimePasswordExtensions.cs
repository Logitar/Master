using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public static class OneTimePasswordExtensions
{
  private const string PurposeKey = "Purpose";
  private const string UserIdKey = "UserId";

  public static Guid GetUserId(this OneTimePassword oneTimePassword)
  {
    CustomAttribute customAttribute = oneTimePassword.CustomAttributes.SingleOrDefault(x => x.Key == UserIdKey)
      ?? throw new InvalidOperationException($"The One-Time Password (OTP) has no '{UserIdKey}' custom attribute.");
    return Guid.Parse(customAttribute.Value);
  }
  public static void SetUserId(this CreateOneTimePasswordPayload payload, User user)
  {
    CustomAttribute? customAttribute = payload.CustomAttributes.SingleOrDefault(x => x.Key == UserIdKey);
    if (customAttribute == null)
    {
      customAttribute = new(UserIdKey, user.Id.ToString());
      payload.CustomAttributes.Add(customAttribute);
    }
    else
    {
      customAttribute.Value = user.Id.ToString();
    }
  }

  public static void EnsurePurpose(this OneTimePassword oneTimePassword, string purpose)
  {
    if (!oneTimePassword.HasPurpose(purpose))
    {
      throw new InvalidOneTimePasswordPurpose(oneTimePassword, purpose);
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
  public static void SetPurpose(this CreateOneTimePasswordPayload payload, string purpose)
  {
    CustomAttribute? customAttribute = payload.CustomAttributes.SingleOrDefault(x => x.Key == PurposeKey);
    if (customAttribute == null)
    {
      customAttribute = new(PurposeKey, purpose);
      payload.CustomAttributes.Add(customAttribute);
    }
    else
    {
      customAttribute.Value = purpose;
    }
  }
}
