namespace Logitar.Master.Application;

/// <summary>
/// The exception thrown when too many results are found.
/// </summary>
public class TooManyResultsException : Exception
{
  /// <summary>
  /// The generic error message.
  /// </summary>
  public const string ErrorMessage = "Too many results were found.";

  /// <summary>
  /// Gets or sets the expected result count.
  /// </summary>
  public int ExpectedCount
  {
    get => (int)Data[nameof(ExpectedCount)]!;
    private set => Data[nameof(ExpectedCount)] = value;
  }
  /// <summary>
  /// Gets or sets the actual result count.
  /// </summary>
  public int ActualCount
  {
    get => (int)Data[nameof(ActualCount)]!;
    private set => Data[nameof(ActualCount)] = value;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TooManyResultsException"/> class.
  /// </summary>
  /// <param name="type">The result type.</param>
  /// <param name="expectedCount">The expected result count.</param>
  /// <param name="actualCount">The actual result count.</param>
  public TooManyResultsException(Type type, int expectedCount, int actualCount) : base(BuildMessage(type, expectedCount, actualCount))
  {
  }

  private static string BuildMessage(Type type, int expectedCount, int actualCount) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(Type), type.GetLongestName())
    .AddData(nameof(ExpectedCount), expectedCount)
    .AddData(nameof(ActualCount), actualCount)
    .Build();
}

/// <summary>
/// The exception thrown when too many results are found.
/// </summary>
/// <typeparam name="T">The result type.</typeparam>
public class TooManyResultsException<T> : TooManyResultsException
{
  /// <summary>
  /// Initializes a new instance of the <see cref="TooManyResultsException"/> class.
  /// </summary>
  /// <param name="expectedCount">The expected result count.</param>
  /// <param name="actualCount">The actual result count.</param>
  public TooManyResultsException(int expectedCount, int actualCount) : base(typeof(T), expectedCount, actualCount)
  {
  }
}
