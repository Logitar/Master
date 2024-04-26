using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;

namespace Logitar.Master.Application.Projects;

public class UniqueKeyAlreadyUsedException : Exception
{
  private const string ErrorMessage = "The specified unique key is already used.";

  public UniqueKeyUnit UniqueKey
  {
    get => new((string)Data[nameof(UniqueKey)]!);
    private set => Data[nameof(UniqueKey)] = value.Value;
  }
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  public UniqueKeyAlreadyUsedException(UniqueKeyUnit uniqueKey, string? propertyName = null) : base(BuildMessage(uniqueKey, propertyName))
  {
    UniqueKey = uniqueKey;
    PropertyName = propertyName ?? nameof(Project.UniqueKey);
  }

  private static string BuildMessage(UniqueKeyUnit uniqueKey, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UniqueKey), uniqueKey.Value)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
