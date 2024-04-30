using FluentValidation;
using Logitar.Master.Application.Accounts.Validators;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

internal class ChangePhoneCommandHandler : IRequestHandler<ChangePhoneCommand, ChangePhoneResult>
{
  private const string ContactVerificationPurpose = "ContactVerification";
  private const string ContactVerificationTemplate = "ContactVerification{ContactType}";

  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly IUserService _userService;

  public ChangePhoneCommandHandler(IMessageService messageService, IOneTimePasswordService oneTimePasswordService, IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _userService = userService;
  }

  public async Task<ChangePhoneResult> Handle(ChangePhoneCommand command, CancellationToken cancellationToken)
  {
    if (command.User == null)
    {
      throw new ArgumentException($"An authenticated '{nameof(command.User)}' is required.", nameof(command));
    }

    ChangePhonePayload payload = command.Payload;
    new ChangePhoneValidator().ValidateAndThrow(payload);

    if (payload.NewPhone != null)
    {
      return await HandleNewPhoneAsync(payload.NewPhone, payload.Locale, command.User, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, command.User, cancellationToken);
    }

    throw new ArgumentException($"The '{nameof(command.Payload)}' is not valid.", nameof(command));
  }

  private async Task<ChangePhoneResult> HandleNewPhoneAsync(AccountPhone newPhone, string locale, User user, CancellationToken cancellationToken)
  {
    Phone phone = newPhone.ToPhone();
    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(user, ContactVerificationPurpose, phone, cancellationToken);
    if (oneTimePassword.Password == null)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.");
    }
    Dictionary<string, string> variables = new()
    {
      ["OneTimePassword"] = oneTimePassword.Password
    };
    string template = ContactVerificationTemplate.Replace("{ContactType}", ContactType.Phone.ToString());
    SentMessages sentMessages = await _messageService.SendAsync(template, phone, locale, variables, cancellationToken);
    SentMessage sentMessage = sentMessages.ToSentMessage(phone);
    OneTimePasswordValidation oneTimePasswordValidation = new(oneTimePassword, sentMessage);
    return new ChangePhoneResult(oneTimePasswordValidation);
  }

  private async Task<ChangePhoneResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, User user, CancellationToken cancellationToken)
  {
    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, ContactVerificationPurpose, cancellationToken);
    Guid userId = oneTimePassword.GetUserId();
    if (userId != user.Id)
    {
      throw new InvalidOneTimePasswordUserException(oneTimePassword, user);
    }
    Phone phone = oneTimePassword.GetPhone();
    phone.IsVerified = true;
    user = await _userService.UpdatePhoneAsync(user.Id, phone, cancellationToken);
    return new ChangePhoneResult(user.ToUserProfile());
  }
}
