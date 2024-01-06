using Logitar.EventSourcing;

namespace Logitar.Master.Application;

public class AggregateNotFoundException : Exception
{
  public const string ErrorMessage = "The specified aggregate could not be found.";

  public string TypeName
  {
    get => (string)Data[nameof(TypeName)]!;
    private set => Data[nameof(TypeName)] = value;
  }
  public string Id
  {
    get => (string)Data[nameof(Id)]!;
    private set => Data[nameof(Id)] = value;
  }

  public AggregateNotFoundException(Type type, AggregateId id) : base(BuildMessage(type, id))
  {
    TypeName = type.GetNamespaceQualifiedName();
    Id = id.Value;
  }

  private static string BuildMessage(Type type, AggregateId id) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TypeName), type.GetNamespaceQualifiedName())
    .AddData(nameof(Id), id)
    .Build();
}

public class AggregateNotFoundException<T> : AggregateNotFoundException where T : AggregateRoot
{
  public AggregateNotFoundException(AggregateId id) : base(typeof(T), id)
  {
  }
}
