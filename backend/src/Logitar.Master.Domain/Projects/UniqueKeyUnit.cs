using FluentValidation;

namespace Logitar.Master.Domain.Projects;

public record UniqueKeyUnit
{
  public const int MaximumLength = 10;

  public string Value { get; }

  public UniqueKeyUnit(string value)
  {
    Value = value.Trim();
    new UniqueKeyValidator().ValidateAndThrow(value);
  }

  public static UniqueKeyUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new UniqueKeyUnit(value.Trim());
  }
}
