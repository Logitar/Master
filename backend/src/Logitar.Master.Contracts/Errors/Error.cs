namespace Logitar.Master.Contracts.Errors;

public record Error
{
  public string Code { get; set; }
  public string Message { get; set; }
  public List<ErrorData> Data { get; set; }

  public Error() : this(string.Empty, string.Empty)
  {
  }

  public Error(string code, string message)
  {
    Code = code;
    Message = message;
    Data = [];
  }

  public Error(string code, string message, IEnumerable<ErrorData> data) : this(code, message)
  {
    Data.AddRange(data);
  }

  public void Add(string key, string value) => Add(new ErrorData(key, value));
  public void Add(KeyValuePair<string, string> data) => Add(new ErrorData(data));
  public void Add(ErrorData data) => Data.Add(data);
}
