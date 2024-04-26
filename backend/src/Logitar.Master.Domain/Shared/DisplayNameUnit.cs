using FluentValidation;

namespace Logitar.Master.Domain.Shared;

public record DisplayNameUnit
{
  public const int MaximumLength = byte.MaxValue;

  public string Value { get; }

  public DisplayNameUnit(string value)
  {
    Value = value.Trim();
    new DisplayNameValidator().ValidateAndThrow(value);
  }

  public static DisplayNameUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new DisplayNameUnit(value.Trim());
  }
}
