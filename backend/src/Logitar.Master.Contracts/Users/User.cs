using Logitar.Master.Contracts.Actors;
using Logitar.Master.Contracts.Sessions;

namespace Logitar.Master.Contracts.Users;

public class User : Aggregate
{
  public string? TenantId { get; set; }

  public string UniqueName { get; set; } = string.Empty;

  public Actor? PasswordChangedBy { get; set; }
  public DateTime? PasswordChangedOn { get; set; }
  public bool HasPassword { get; set; }

  public string? FullName { get; set; }

  public DateTime? AuthenticatedOn { get; set; }

  public List<Session> Sessions { get; set; } = [];

  public override string ToString() => $"{FullName ?? UniqueName} | {base.ToString()}";
}
