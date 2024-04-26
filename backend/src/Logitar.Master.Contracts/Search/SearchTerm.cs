namespace Logitar.Master.Contracts.Search;

public record SearchTerm
{
  public string Value { get; set; }

  public SearchTerm() : this(string.Empty)
  {
  }

  public SearchTerm(string value)
  {
    Value = value;
  }
}
