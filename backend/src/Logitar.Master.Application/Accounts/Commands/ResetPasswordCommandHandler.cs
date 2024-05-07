using FluentValidation;
using Logitar.Master.Application.Accounts.Events;
using Logitar.Master.Application.Accounts.Validators;
using Logitar.Master.Application.Constants;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

internal class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResult>
{
  private readonly IMessageService _messageService;
  private readonly IPublisher _publisher;
  private readonly IRealmService _realmService;
  private readonly ISessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public ResetPasswordCommandHandler(IMessageService messageService, IPublisher publisher,
    IRealmService realmService, ISessionService sessionService, ITokenService tokenService, IUserService userService)
  {
    _messageService = messageService;
    _publisher = publisher;
    _realmService = realmService;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<ResetPasswordResult> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
  {
    Realm realm = await _realmService.FindAsync(cancellationToken);

    ResetPasswordPayload payload = command.Payload;
    new ResetPasswordValidator(realm.PasswordSettings).ValidateAndThrow(payload);

    if (!string.IsNullOrWhiteSpace(payload.EmailAddress))
    {
      return await HandleEmailAddressAsync(payload.EmailAddress, payload.Locale, cancellationToken);
    }
    else if (payload.Token != null && payload.NewPassword != null)
    {
      return await HandleNewPasswordAsync(payload.Token, payload.NewPassword, command.CustomAttributes, cancellationToken);
    }

    throw new ArgumentException($"The '{nameof(command.Payload)}' is not valid.", nameof(command));
  }

  private async Task<ResetPasswordResult> HandleEmailAddressAsync(string emailAddress, string locale, CancellationToken cancellationToken)
  {
    SentMessages sentMessages;

    User? user = await _userService.FindAsync(emailAddress, cancellationToken);
    if (user != null)
    {
      CreatedToken token = await _tokenService.CreateAsync(user.GetSubject(), TokenTypes.PasswordRecovery, cancellationToken);
      Dictionary<string, string> variables = new()
      {
        ["Token"] = token.Token
      };
      sentMessages = await _messageService.SendAsync(Templates.PasswordRecovery, user, ContactType.Email, locale, variables, cancellationToken);
    }
    else
    {
      sentMessages = new([Guid.NewGuid()]);
    }

    SentMessage sentMessage = sentMessages.ToSentMessage(new Email(emailAddress));
    return ResetPasswordResult.RecoveryLinkSent(sentMessage);
  }

  private async Task<ResetPasswordResult> HandleNewPasswordAsync(string token, string newPassword, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(token, TokenTypes.PasswordRecovery, cancellationToken);
    if (validatedToken.Subject == null)
    {
      throw new InvalidOperationException($"The claim '{validatedToken.Subject}' is required.");
    }
    Guid userId = Guid.Parse(validatedToken.Subject);
    _ = await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
    User user = await _userService.ResetPasswordAsync(userId, newPassword, cancellationToken);

    return await EnsureProfileIsCompleted(user, customAttributes, cancellationToken);
  }

  private async Task<ResetPasswordResult> EnsureProfileIsCompleted(User user, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    if (!user.IsProfileCompleted())
    {
      CreatedToken token = await _tokenService.CreateAsync(user.GetSubject(), TokenTypes.Profile, cancellationToken);
      return ResetPasswordResult.RequireProfileCompletion(token);
    }

    Session session = await _sessionService.CreateAsync(user, customAttributes, cancellationToken);
    await _publisher.Publish(new UserSignedInEvent(session), cancellationToken);
    return ResetPasswordResult.Succeed(session);
  }
}
