namespace Logitar.Master.Settings;

internal record BearerTokenSettings
{
  public int LifetimeSeconds { get; set; }
  public string TokenType { get; set; }

  public BearerTokenSettings() : this(string.Empty)
  {
  }

  public BearerTokenSettings(string tokenType)
  {
    TokenType = tokenType;
  }
}
