﻿using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

public interface ITokenService
{
  Task<CreatedToken> CreateAsync(string? subject, Email email, string type, CancellationToken cancellationToken = default);
}