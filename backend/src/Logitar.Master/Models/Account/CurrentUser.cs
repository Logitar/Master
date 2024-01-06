using Logitar.Master.Contracts.Users;

namespace Logitar.Master.Models.Account;

public record CurrentUser
{
  public string DisplayName { get; }
  public string? EmailAddress { get; }
  public string? PictureUrl { get; }

  public CurrentUser(User user)
  {
    DisplayName = user.FullName ?? user.UniqueName;
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }
}
