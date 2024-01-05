using Logitar.Master.Contracts.Users;

namespace Logitar.Master.Contracts.Sessions;

public class Session : Aggregate
{
  public bool IsActive { get; set; }

  public User? User { get; set; }
}
