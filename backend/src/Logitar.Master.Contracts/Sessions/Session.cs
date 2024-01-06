using Logitar.Master.Contracts.Actors;
using Logitar.Master.Contracts.Users;

namespace Logitar.Master.Contracts.Sessions;

public class Session : Aggregate
{
  public bool IsPersistent { get; set; }

  public Actor? SignedOutBy { get; set; }
  public DateTime? SignedOutOn { get; set; }
  public bool IsActive { get; set; }

  public string? RefreshToken { get; set; }

  public User? User { get; set; }
}
