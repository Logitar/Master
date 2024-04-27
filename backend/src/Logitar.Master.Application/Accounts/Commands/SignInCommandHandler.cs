using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

internal class SignInCommandHandler : IRequestHandler<SignInCommand, SignInCommandResult>
{
  private const string AuthenticationTokenType = "auth+jwt";
  private const string PasswordlessTemplate = "AccountAuthentication";

  private readonly IMessageService _messageService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public SignInCommandHandler(IMessageService messageService, ITokenService tokenService, IUserService userService)
  {
    _messageService = messageService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<SignInCommandResult> Handle(SignInCommand command, CancellationToken cancellationToken)
  {
    SignInPayload payload = command.Payload;
    // TODO(fpion): validation payload

    if (payload.Credentials != null)
    {
      return await HandleCredentialsAsync(payload.Credentials, payload.Locale, cancellationToken);
    }

    throw new InvalidOperationException($"The {nameof(SignInPayload)} is not valid.");
  }

  private async Task<SignInCommandResult> HandleCredentialsAsync(Credentials credentials, string locale, CancellationToken cancellationToken)
  {
    User? user = await _userService.FindAsync(credentials.EmailAddress, cancellationToken);
    if (user == null || !user.HasPassword)
    {
      Email email = user?.Email ?? new(credentials.EmailAddress);
      CreatedToken token = await _tokenService.CreateAsync(user?.GetSubject(), email, AuthenticationTokenType, cancellationToken);
      Dictionary<string, string> variables = new()
      {
        ["Token"] = token.Token
      };
      SentMessages sentMessages = user == null
        ? await _messageService.SendAsync(PasswordlessTemplate, email, locale, variables, cancellationToken)
        : await _messageService.SendAsync(PasswordlessTemplate, user, locale, variables, cancellationToken);
      SentMessage sentMessage = sentMessages.ToSentMessage(email);
      return SignInCommandResult.AuthenticationLinkSent(sentMessage);
    }
    else if (credentials.Password == null)
    {
      return SignInCommandResult.RequirePassword();
    }

    throw new NotImplementedException(); // TODO(fpion): implement
  }
}
