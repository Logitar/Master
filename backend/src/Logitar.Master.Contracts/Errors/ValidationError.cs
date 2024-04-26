namespace Logitar.Master.Contracts.Errors;

public record ValidationError
{
  public string Code { get; set; }
  public string Message { get; set; }
  public List<PropertyError> Failures { get; set; }

  public ValidationError() : this(code: "Validation", message: $"Validation failed. See {nameof(Failures)} for more detail.")
  {
  }

  public ValidationError(string code, string message)
  {
    Code = code;
    Message = message;
    Failures = [];
  }

  public ValidationError(IEnumerable<PropertyError> failures) : this()
  {
    Failures.AddRange(failures);
  }

  public ValidationError(string code, string message, IEnumerable<PropertyError> failures) : this(code, message)
  {
    Failures.AddRange(failures);
  }

  public void Add(PropertyError failure) => Failures.Add(failure);
}
