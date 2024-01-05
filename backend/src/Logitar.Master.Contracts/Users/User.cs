using Logitar.Master.Contracts.Sessions;

namespace Logitar.Master.Contracts.Users;

public class User : Aggregate
{
  public string? TenantId { get; set; }

  public string UniqueName { get; set; } = string.Empty;

  public string? FullName { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<Session> Sessions { get; set; } = [];

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
