using Logitar.Master.Contracts.Accounts;

namespace Logitar.Master.Models.Account;

public record GetTokenPayload : SignInPayload
{
  [JsonPropertyName("refresh_token")]
  public string? RefreshToken { get; set; }
}
