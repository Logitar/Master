namespace Logitar.Master.Application.Sessions;

internal record RefreshToken
{
  public const string Prefix = "RT";
  public const int SecretLength = 32;
  public const char Separator = ':';

  public string Id { get; }
  public byte[] Secret { get; }

  public RefreshToken(string id, byte[] secret)
  {
    if (secret.Length != SecretLength)
    {
      throw new ArgumentException($"The secret length should be {SecretLength} bytes.", nameof(secret));
    }

    Id = id;
    Secret = secret;
  }

  public static RefreshToken Decode(string value)
  {
    string[] values = value.Split(Separator);
    if (values.Length != 3 || values.First() != Prefix)
    {
      throw new ArgumentException($"The value '{value}' is not a valid refresh token.", nameof(value));
    }

    return new RefreshToken(values[1], Convert.FromBase64String(values[2].FromUriSafeBase64()));
  }

  public string Encode() => string.Join(Separator, Prefix, Id, Convert.ToBase64String(Secret).ToUriSafeBase64());
}
