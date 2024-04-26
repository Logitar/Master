namespace Logitar.Master.Contracts.Errors;

public record ErrorData
{
  public string Key { get; set; }
  public string Value { get; set; }

  public ErrorData() : this(string.Empty, string.Empty)
  {
  }

  public ErrorData(KeyValuePair<string, string> data) : this(data.Key, data.Value)
  {
  }

  public ErrorData(string key, string value)
  {
    Key = key;
    Value = value;
  }
}
