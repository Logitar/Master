namespace Logitar.Master.Contracts.Errors;

public record PropertyError : Error
{
  public string? PropertyName { get; set; }
  public object? AttemptedValue { get; set; }

  public PropertyError() : base()
  {
  }

  public PropertyError(string code, string message, string? propertyName = null, object? attemptedValue = null) : base(code, message)
  {
    PropertyName = propertyName;
    AttemptedValue = attemptedValue;
  }
}
