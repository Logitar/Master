using FluentValidation;

namespace Logitar.Master.Domain.Shared;

public record DescriptionUnit
{
  public string Value { get; }

  public DescriptionUnit(string value)
  {
    Value = value.Trim();
    new DescriptionValidator().ValidateAndThrow(value);
  }

  public static DescriptionUnit? TryCreate(string? value)
  {
    return string.IsNullOrWhiteSpace(value) ? null : new DescriptionUnit(value.Trim());
  }
}
