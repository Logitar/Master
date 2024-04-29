using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Models.Account;

public record CurrentUser
{
  public string DisplayName { get; set; }
  public string? EmailAddress { get; set; }
  public string? PictureUrl { get; set; }

  public CurrentUser() : this(string.Empty)
  {
  }

  public CurrentUser(string displayName)
  {
    DisplayName = displayName;
  }

  public CurrentUser(User user) : this(user.FullName ?? user.UniqueName)
  {
    EmailAddress = user.Email?.Address;
    PictureUrl = user.Picture;
  }
}
