using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;

namespace Logitar.Master.Extensions;

internal static class ClaimsExtensions
{
  public static ClaimsIdentity CreateClaimsIdentity(this ApiKey apiKey, string? authenticationType = null)
  {
    ClaimsIdentity identity = new(authenticationType);

    identity.AddClaim(new(Rfc7519ClaimNames.Subject, apiKey.Id.ToString()));
    identity.AddClaim(new(Rfc7519ClaimNames.FullName, apiKey.DisplayName));

    if (apiKey.AuthenticatedOn.HasValue)
    {
      identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, apiKey.AuthenticatedOn.Value));
    }

    return identity;
  }

  public static ClaimsIdentity CreateClaimsIdentity(this Session session, string? authenticationType = null)
  {
    ClaimsIdentity identity = session.User.CreateClaimsIdentity(authenticationType);

    identity.AddClaim(new Claim(Rfc7519ClaimNames.SessionId, session.Id.ToString()));

    return identity;
  }
  public static ClaimsIdentity CreateClaimsIdentity(this User user, string? authenticationType = null)
  {
    ClaimsIdentity identity = new(authenticationType);

    identity.AddClaim(new Claim(Rfc7519ClaimNames.Subject, user.Id.ToString()));
    identity.AddClaim(new Claim(Rfc7519ClaimNames.Username, user.UniqueName));

    if (user.FullName != null)
    {
      if (user.FirstName != null)
      {
        identity.AddClaim(new Claim(Rfc7519ClaimNames.FirstName, user.FirstName));
      }
      if (user.MiddleName != null)
      {
        identity.AddClaim(new Claim(Rfc7519ClaimNames.MiddleName, user.MiddleName));
      }
      if (user.LastName != null)
      {
        identity.AddClaim(new Claim(Rfc7519ClaimNames.LastName, user.LastName));
      }

      identity.AddClaim(new Claim(Rfc7519ClaimNames.FullName, user.FullName));
    }

    if (user.Email != null)
    {
      identity.AddClaim(new Claim(Rfc7519ClaimNames.EmailAddress, user.Email.Address));
      identity.AddClaim(new Claim(Rfc7519ClaimNames.IsEmailVerified, user.Email.IsVerified.ToString().ToLower(), ClaimValueTypes.Boolean));
    }

    if (user.Picture != null)
    {
      identity.AddClaim(new Claim(Rfc7519ClaimNames.Picture, user.Picture));
    }

    if (user.AuthenticatedOn.HasValue)
    {
      identity.AddClaim(ClaimHelper.Create(Rfc7519ClaimNames.AuthenticationTime, user.AuthenticatedOn.Value));
    }

    foreach (Role role in user.Roles)
    {
      identity.AddClaim(new Claim(Rfc7519ClaimNames.Roles, role.UniqueName));
    }

    return identity;
  }
}
