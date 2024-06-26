﻿using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public interface IUserService
{
  Task<User> AuthenticateAsync(User user, string password, CancellationToken cancellationToken = default);
  Task<User> AuthenticateAsync(string uniqueName, string password, CancellationToken cancellationToken = default);
  Task<User> CompleteProfileAsync(Guid userId, CompleteProfilePayload payload, Phone? phone = null, CancellationToken cancellationToken = default);
  Task<User> CreateAsync(Email email, CancellationToken cancellationToken = default);
  Task<User?> FindAsync(string uniqueName, CancellationToken cancellationToken = default);
  Task<User?> FindAsync(Guid userId, CancellationToken cancellationToken = default);
  Task<User> ResetPasswordAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default);
  Task<User> SaveProfileAsync(Guid userId, SaveProfilePayload payload, CancellationToken cancellationToken = default);
  Task<User?> SignOutAsync(Guid userId, CancellationToken cancellationToken = default);
  Task<User> UpdateEmailAsync(Guid userId, Email email, CancellationToken cancellationToken = default);
  Task<User> UpdatePhoneAsync(Guid userId, Phone phone, CancellationToken cancellationToken = default);
}
