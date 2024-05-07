namespace Logitar.Master.Application.Constants;

internal static class TokenTypes
{
  public const string Authentication = "auth+jwt";
  public const string PasswordRecovery = "reset_password+jwt";
  public const string Profile = "profile+jwt";
}
