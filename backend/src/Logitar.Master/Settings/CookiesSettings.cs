namespace Logitar.Master.Settings;

internal record CookiesSettings
{
  public RefreshTokenCookieSettings RefreshToken { get; set; } = new();
  public SessionCookieSettings Session { get; set; } = new();
}
