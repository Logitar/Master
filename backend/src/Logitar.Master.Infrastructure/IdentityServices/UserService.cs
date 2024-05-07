using Logitar.Identity.Contracts;
using Logitar.Master.Application.Accounts;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Infrastructure.IdentityServices;

internal class UserService : IUserService
{
  private readonly IUserClient _userClient;

  public UserService(IUserClient userClient)
  {
    _userClient = userClient;
  }

  public async Task<User> AuthenticateAsync(User user, string password, CancellationToken cancellationToken)
  {
    return await AuthenticateAsync(user.UniqueName, password, cancellationToken);
  }
  public async Task<User> AuthenticateAsync(string uniqueName, string password, CancellationToken cancellationToken)
  {
    AuthenticateUserPayload payload = new(uniqueName, password);
    RequestContext context = new(uniqueName, cancellationToken);
    return await _userClient.AuthenticateAsync(payload, context);
  }

  public async Task<User> CompleteProfileAsync(Guid userId, CompleteProfilePayload profile, Phone? phone, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = profile.ToUpdateUserPayload();
    if (profile.Password != null)
    {
      payload.Password = new ChangePasswordPayload(profile.Password);
    }
    if (phone != null)
    {
      payload.Phone = new Modification<PhonePayload>(phone.ToPhonePayload());
    }
    payload.CompleteProfile();
    payload.SetMultiFactorAuthenticationMode(profile.MultiFactorAuthenticationMode);
    RequestContext context = new(userId.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(userId, payload, context) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
  }

  public async Task<User> CreateAsync(Email email, CancellationToken cancellationToken)
  {
    CreateUserPayload payload = new(email.Address)
    {
      Email = new EmailPayload(email.Address, email.IsVerified)
    };
    RequestContext context = new(cancellationToken);
    return await _userClient.CreateAsync(payload, context);
  }

  public async Task<User?> FindAsync(string uniqueName, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(id: null, uniqueName, identifier: null, context);
  }

  public async Task<User?> FindAsync(Guid userId, CancellationToken cancellationToken)
  {
    RequestContext context = new(cancellationToken);
    return await _userClient.ReadAsync(userId, uniqueName: null, identifier: null, context);
  }

  public async Task<User> SaveProfileAsync(Guid userId, SaveProfilePayload profile, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = profile.ToUpdateUserPayload();
    RequestContext context = new(userId.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(userId, payload, context) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
  }

  public async Task<User?> SignOutAsync(Guid userId, CancellationToken cancellationToken)
  {
    RequestContext context = new(userId.ToString(), cancellationToken);
    return await _userClient.SignOutAsync(userId, context);
  }

  public async Task<User> UpdateEmailAsync(Guid userId, Email email, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Email = new Modification<EmailPayload>(email.ToEmailPayload())
    };
    RequestContext context = new(userId.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(userId, payload, context) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
  }

  public async Task<User> UpdatePhoneAsync(Guid userId, Phone phone, CancellationToken cancellationToken)
  {
    UpdateUserPayload payload = new()
    {
      Phone = new Modification<PhonePayload>(phone.ToPhonePayload())
    };
    RequestContext context = new(userId.ToString(), cancellationToken);
    return await _userClient.UpdateAsync(userId, payload, context) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
  }
}
