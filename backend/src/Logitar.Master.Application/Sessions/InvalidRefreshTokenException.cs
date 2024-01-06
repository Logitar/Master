using Logitar.Identity.Domain.Shared;

namespace Logitar.Master.Application.Sessions;

internal class InvalidRefreshTokenException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified refresh token is not valid.";

  public string RefreshToken
  {
    get => (string)Data[nameof(RefreshToken)]!;
    private set => Data[nameof(RefreshToken)] = value;
  }

  public InvalidRefreshTokenException(string refreshToken, Exception innerException) : base(BuildMessage(refreshToken), innerException)
  {
    RefreshToken = refreshToken;
  }

  private static string BuildMessage(string refreshToken) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(RefreshToken), refreshToken)
    .Build();
}
