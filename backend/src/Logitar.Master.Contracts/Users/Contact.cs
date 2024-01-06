using Logitar.Master.Contracts.Actors;

namespace Logitar.Master.Contracts.Users;

public abstract record Contact
{
  public Actor? VerifiedBy { get; set; }
  public DateTime? VerifiedOn { get; set; }
  public bool IsVerified { get; set; }
}
