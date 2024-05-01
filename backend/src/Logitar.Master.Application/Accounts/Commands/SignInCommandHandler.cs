using Logitar.Master.Application.Accounts.Events;
using Logitar.Master.Application.Constants;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

internal class SignInCommandHandler : IRequestHandler<SignInCommand, SignInCommandResult>
{
  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly IPublisher _publisher;
  private readonly ISessionService _sessionService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public SignInCommandHandler(IMessageService messageService, IOneTimePasswordService oneTimePasswordService,
    IPublisher publisher, ISessionService sessionService, ITokenService tokenService, IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _publisher = publisher;
    _sessionService = sessionService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<SignInCommandResult> Handle(SignInCommand command, CancellationToken cancellationToken)
  {
    SignInPayload payload = command.Payload;

    if (payload.Credentials != null)
    {
      return await HandleCredentialsAsync(payload.Credentials, payload.Locale, command.CustomAttributes, cancellationToken);
    }
    else if (!string.IsNullOrWhiteSpace(payload.AuthenticationToken))
    {
      return await HandleAuthenticationTokenAsync(payload.AuthenticationToken, command.CustomAttributes, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, command.CustomAttributes, cancellationToken);
    }
    else if (payload.Profile != null)
    {
      return await CompleteProfileAsync(payload.Profile, command.CustomAttributes, cancellationToken);
    }

    throw new ArgumentException($"The '{nameof(command.Payload)}' is not valid.", nameof(command));
  }

  private async Task<SignInCommandResult> HandleCredentialsAsync(Credentials credentials, string locale, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    User? user = await _userService.FindAsync(credentials.EmailAddress, cancellationToken);
    if (user == null || !user.HasPassword)
    {
      Email email = user?.Email ?? new(credentials.EmailAddress);
      CreatedToken token = await _tokenService.CreateAsync(user?.GetSubject(), email, TokenTypes.Authentication, cancellationToken);
      Dictionary<string, string> variables = new()
      {
        ["Token"] = token.Token
      };
      SentMessages sentMessages = user == null
        ? await _messageService.SendAsync(Templates.AccountAuthentication, email, locale, variables, cancellationToken)
        : await _messageService.SendAsync(Templates.AccountAuthentication, user, locale, variables, cancellationToken);
      SentMessage sentMessage = sentMessages.ToSentMessage(email);
      return SignInCommandResult.AuthenticationLinkSent(sentMessage);
    }
    else if (credentials.Password == null)
    {
      return SignInCommandResult.RequirePassword();
    }

    MultiFactorAuthenticationMode? mfaMode = user.GetMultiFactorAuthenticationMode();
    if (mfaMode == MultiFactorAuthenticationMode.None && user.IsProfileCompleted())
    {
      Session session = await _sessionService.SignInAsync(user, credentials.Password, customAttributes, cancellationToken);
      await _publisher.Publish(new UserSignedInEvent(session), cancellationToken);
      return SignInCommandResult.Succeed(session);
    }
    else
    {
      user = await _userService.AuthenticateAsync(user, credentials.Password, cancellationToken);
    }

    return mfaMode switch
    {
      MultiFactorAuthenticationMode.Email => await SendMultiFactorAuthenticationMessageAsync(user, ContactType.Email, locale, cancellationToken),
      MultiFactorAuthenticationMode.Phone => await SendMultiFactorAuthenticationMessageAsync(user, ContactType.Phone, locale, cancellationToken),
      _ => await EnsureProfileIsCompleted(user, customAttributes, cancellationToken),
    };
  }
  private async Task<SignInCommandResult> SendMultiFactorAuthenticationMessageAsync(User user, ContactType contactType, string locale, CancellationToken cancellationToken)
  {
    Contact contact = contactType switch
    {
      ContactType.Email => user.Email ?? throw new ArgumentException($"The user 'Id={user.Id}' has no email.", nameof(user)),
      ContactType.Phone => user.Phone ?? throw new ArgumentException($"The user 'Id={user.Id}' has no phone.", nameof(user)),
      _ => throw new ArgumentException($"The contact type '{contactType}' is not supported.", nameof(contactType)),
    };
    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(user, Purposes.MultiFactorAuthentication, cancellationToken);
    if (oneTimePassword.Password == null)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.");
    }
    Dictionary<string, string> variables = new()
    {
      ["OneTimePassword"] = oneTimePassword.Password
    };
    string template = Templates.GetMultiFactorAuthentication(contactType);
    SentMessages sentMessages = await _messageService.SendAsync(template, user, locale, variables, cancellationToken);
    SentMessage sentMessage = sentMessages.ToSentMessage(contact);
    return SignInCommandResult.RequireOneTimePasswordValidation(oneTimePassword, sentMessage);
  }

  private async Task<SignInCommandResult> HandleAuthenticationTokenAsync(string token, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(token, TokenTypes.Authentication, cancellationToken);
    Email? email = validatedToken.Email == null ? null : new(validatedToken.Email.Address)
    {
      IsVerified = true
    };

    User user;
    if (validatedToken.Subject == null)
    {
      if (email == null)
      {
        throw new InvalidOperationException($"The '{nameof(validatedToken.Email)}' claims are required.");
      }

      user = await _userService.CreateAsync(email, cancellationToken);
    }
    else
    {
      Guid userId = Guid.Parse(validatedToken.Subject);
      user = await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");

      if (email != null && (user.Email == null || user.Email.Address != email.Address || user.Email.IsVerified != email.IsVerified))
      {
        user = await _userService.UpdateEmailAsync(user.Id, email, cancellationToken);
      }
    }

    return await EnsureProfileIsCompleted(user, customAttributes, cancellationToken);
  }

  private async Task<SignInCommandResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, Purposes.MultiFactorAuthentication, cancellationToken);
    Guid userId = oneTimePassword.GetUserId();
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");

    return await EnsureProfileIsCompleted(user, customAttributes, cancellationToken);
  }

  private async Task<SignInCommandResult> CompleteProfileAsync(CompleteProfilePayload payload, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(payload.Token, TokenTypes.Profile, cancellationToken);
    if (validatedToken.Subject == null)
    {
      throw new InvalidOperationException($"The claim '{validatedToken.Subject}' is required.");
    }
    Guid userId = Guid.Parse(validatedToken.Subject);
    User user = await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
    user = await _userService.SaveProfileAsync(user.Id, payload, cancellationToken);

    return await EnsureProfileIsCompleted(user, customAttributes, cancellationToken);
  }

  private async Task<SignInCommandResult> EnsureProfileIsCompleted(User user, IEnumerable<CustomAttribute> customAttributes, CancellationToken cancellationToken)
  {
    if (!user.IsProfileCompleted())
    {
      CreatedToken token = await _tokenService.CreateAsync(user.GetSubject(), TokenTypes.Profile, cancellationToken);
      return SignInCommandResult.RequireProfileCompletion(token);
    }

    Session session = await _sessionService.CreateAsync(user, customAttributes, cancellationToken);
    await _publisher.Publish(new UserSignedInEvent(session), cancellationToken);
    return SignInCommandResult.Succeed(session);
  }
}
