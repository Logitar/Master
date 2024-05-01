using FluentValidation;
using Logitar.Master.Application.Accounts.Validators;
using Logitar.Master.Application.Constants;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

internal class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand, VerifyPhoneResult>
{
  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public VerifyPhoneCommandHandler(IMessageService messageService, IOneTimePasswordService oneTimePasswordService,
    ITokenService tokenService, IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<VerifyPhoneResult> Handle(VerifyPhoneCommand command, CancellationToken cancellationToken)
  {
    VerifyPhonePayload payload = command.Payload;
    new VerifyPhoneValidator().ValidateAndThrow(payload);

    User user = await FindUserAsync(payload.ProfileCompletionToken, cancellationToken);

    if (payload.NewPhone != null)
    {
      return await HandleNewPhoneAsync(payload.NewPhone, payload.Locale, user, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      VerifyPhoneResult result = await HandleOneTimePasswordAsync(payload.OneTimePassword, user, cancellationToken);
      await InvalidateTokenAsync(payload.ProfileCompletionToken, cancellationToken);
      return result;
    }

    throw new ArgumentException($"The '{nameof(command.Payload)}' is not valid.", nameof(command));
  }

  private async Task<User> FindUserAsync(string profileCompletionToken, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(profileCompletionToken, consume: false, TokenTypes.Profile, cancellationToken);
    if (validatedToken.Subject == null)
    {
      throw new InvalidOperationException($"The claim '{validatedToken.Subject}' is required.");
    }
    Guid userId = Guid.Parse(validatedToken.Subject);
    return await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
  }

  private async Task InvalidateTokenAsync(string profileCompletionToken, CancellationToken cancellationToken)
  {
    try
    {
      _ = await _tokenService.ValidateAsync(profileCompletionToken, consume: true, TokenTypes.Profile, cancellationToken);
    }
    catch (Exception)
    {
    }
  }

  private async Task<VerifyPhoneResult> HandleNewPhoneAsync(AccountPhone newPhone, string locale, User user, CancellationToken cancellationToken)
  {
    Phone phone = newPhone.ToPhone();
    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(user, Purposes.ContactVerification, phone, cancellationToken);
    if (oneTimePassword.Password == null)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.");
    }
    Dictionary<string, string> variables = new()
    {
      ["OneTimePassword"] = oneTimePassword.Password
    };
    string template = Templates.GetContactVerification(ContactType.Phone);
    SentMessages sentMessages = await _messageService.SendAsync(template, phone, locale, variables, cancellationToken);
    SentMessage sentMessage = sentMessages.ToSentMessage(phone);
    OneTimePasswordValidation oneTimePasswordValidation = new(oneTimePassword, sentMessage);
    return new VerifyPhoneResult(oneTimePasswordValidation);
  }

  private async Task<VerifyPhoneResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, User user, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, Purposes.ContactVerification, cancellationToken);
    Guid userId = oneTimePassword.GetUserId();
    if (userId != user.Id)
    {
      throw new InvalidOneTimePasswordUserException(oneTimePassword, user);
    }
    Phone phone = oneTimePassword.GetPhone();
    phone.IsVerified = true;
    CreatedToken profileCompletion = await _tokenService.CreateAsync(user.GetSubject(), phone, TokenTypes.Profile, cancellationToken);
    return new VerifyPhoneResult(profileCompletion.Token);
  }
}
