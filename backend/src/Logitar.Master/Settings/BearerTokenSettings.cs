namespace Logitar.Master.Settings;

internal record BearerTokenSettings
{
  public int LifetimeSeconds { get; set; }
  public string Secret { get; set; }
  public string TokenType { get; set; }

  public BearerTokenSettings() : this(string.Empty, string.Empty)
  {
  }

  public BearerTokenSettings(string secret, string tokenType)
  {
    Secret = secret;
    TokenType = tokenType;
  }
}
