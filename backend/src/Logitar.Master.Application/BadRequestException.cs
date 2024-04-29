using Logitar.Portal.Contracts.Errors;

namespace Logitar.Master.Application;

public abstract class BadRequestException : Exception
{
  public abstract Error Error { get; }

  protected BadRequestException(string? message = null, Exception? innerException = null) : base(message, innerException)
  {
  }
}
