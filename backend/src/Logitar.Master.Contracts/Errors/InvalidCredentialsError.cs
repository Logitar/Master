using Logitar.Portal.Contracts.Errors;

namespace Logitar.Master.Contracts.Errors;

public record InvalidCredentialsError : Error
{
  public InvalidCredentialsError() : base("InvalidCredentials", "The specified credentials did not match.")
  {
  }
}
