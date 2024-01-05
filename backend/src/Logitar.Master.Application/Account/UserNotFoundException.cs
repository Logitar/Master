using Logitar.Identity.Domain.Shared;

namespace Logitar.Master.Application.Account;

public class UserNotFoundException : InvalidCredentialsException
{
  public new const string ErrorMessage = "The specified user could not be found.";

  public string? TenantId
  {
    get => (string?)Data[nameof(TenantId)];
    private set => Data[nameof(TenantId)] = value;
  }
  public string UniqueName
  {
    get => (string)Data[nameof(UniqueName)]!;
    private set => Data[nameof(UniqueName)] = value;
  }

  public UserNotFoundException(string? tenantId, string uniqueName) : base(BuildMessage(tenantId, uniqueName))
  {
    TenantId = tenantId;
    UniqueName = uniqueName;
  }

  private static string BuildMessage(string? tenantId, string uniqueName) => new ErrorMessageBuilder(ErrorMessage)
    .AddData(nameof(TenantId), tenantId ?? "<null>")
    .AddData(nameof(UniqueName), uniqueName)
    .Build();
}
