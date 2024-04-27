using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

internal static class UserExtensions
{
  public static string GetSubject(this User user) => user.Id.ToString();
}
