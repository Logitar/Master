using Logitar.Master.Domain.Projects;

namespace Logitar.Master.Application.Projects;

/// <summary>
/// The exception thrown when a project unique key conflict occurs.
/// </summary>
public class UniqueKeyAlreadyUsedException : Exception
{
  /// <summary>
  /// The generic error message.
  /// </summary>
  public const string ErrorMessage = "The specified unique key is already used.";

  /// <summary>
  /// Gets or sets the conflicting unique key.
  /// </summary>
  public UniqueKeyUnit UniqueKey
  {
    get => new((string)Data[nameof(UniqueKey)]!);
    private set => Data[nameof(UniqueKey)] = value.Value;
  }
  /// <summary>
  /// Gets or sets the name of the property.
  /// </summary>
  public string? PropertyName
  {
    get => (string?)Data[nameof(PropertyName)];
    private set => Data[nameof(PropertyName)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="UniqueKeyAlreadyUsedException"/> class.
  /// </summary>
  /// <param name="uniqueKey">The conflicting unique key.</param>
  /// <param name="propertyName">The name of the property.</param>
  /// <param name="innerException">The inner exception.</param>
  public UniqueKeyAlreadyUsedException(UniqueKeyUnit uniqueKey, string? propertyName = null, Exception? innerException = null)
    : base(BuildMessage(uniqueKey, propertyName), innerException)
  {
    UniqueKey = uniqueKey;
    PropertyName = propertyName;
  }

  private static string BuildMessage(UniqueKeyUnit uniqueKey, string? propertyName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(UniqueKey), uniqueKey.Value)
    .AddData(nameof(PropertyName), propertyName, "<null>")
    .Build();
}
