using Logitar.Master.Application.Constants;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

internal class ChangePhoneCommandHandler : IRequestHandler<ChangePhoneCommand, ChangePhoneResult>
{
  private const string ContactVerificationPurpose = "ContactVerification";
  private const string ContactVerificationTemplate = "ContactVerification{ContactType}";

  private readonly IMessageService _messageService;
  private readonly IOneTimePasswordService _oneTimePasswordService;
  private readonly ITokenService _tokenService;
  private readonly IUserService _userService;

  public ChangePhoneCommandHandler(IMessageService messageService, IOneTimePasswordService oneTimePasswordService,
    ITokenService tokenService, IUserService userService)
  {
    _messageService = messageService;
    _oneTimePasswordService = oneTimePasswordService;
    _tokenService = tokenService;
    _userService = userService;
  }

  public async Task<ChangePhoneResult> Handle(ChangePhoneCommand command, CancellationToken cancellationToken)
  {
    ChangePhonePayload payload = command.Payload; // TODO(fpion): validate payload ; ProfileCompletionToken is required if command.User == null

    if (payload.Phone != null)
    {
      return await HandlePhoneAsync(payload.Phone, payload.Locale, payload.ProfileCompletionToken, command.User, cancellationToken);
    }
    else if (payload.OneTimePassword != null)
    {
      return await HandleOneTimePasswordAsync(payload.OneTimePassword, payload.ProfileCompletionToken, command.User, cancellationToken);
    }

    throw new ArgumentException($"The '{nameof(command)}.{nameof(command.Payload)}' is not valid.", nameof(command));
  }

  private async Task<ChangePhoneResult> HandlePhoneAsync(AccountPhone phone, string locale, string? profileCompletionToken, User? user, CancellationToken cancellationToken)
  {
    if (user == null)
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(profileCompletionToken, nameof(profileCompletionToken));
      user = await FindUserAsync(profileCompletionToken, cancellationToken);
    }

    Phone contact = new(phone.CountryCode, phone.Number, extension: null, e164Formatted: "TODO");
    OneTimePassword oneTimePassword = await _oneTimePasswordService.CreateAsync(user, contact, ContactVerificationPurpose, cancellationToken);
    if (oneTimePassword.Password == null)
    {
      throw new InvalidOperationException($"The One-Time Password (OTP) 'Id={oneTimePassword.Id}' has no password.");
    }
    Dictionary<string, string> variables = new()
    {
      ["OneTimePassword"] = oneTimePassword.Password
    };
    string template = ContactVerificationTemplate.Replace("{ContactType}", ContactType.Phone.ToString());
    SentMessages sentMessages = await _messageService.SendAsync(template, contact, locale, variables, cancellationToken);
    SentMessage sentMessage = sentMessages.ToSentMessage(contact);
    OneTimePasswordValidation oneTimePasswordValidation = new(oneTimePassword, sentMessage);

    return new ChangePhoneResult(oneTimePasswordValidation);
  }

  private async Task<ChangePhoneResult> HandleOneTimePasswordAsync(OneTimePasswordPayload payload, string? profileCompletionToken, User? user, CancellationToken cancellationToken)
  {
    if (user == null)
    {
      ArgumentException.ThrowIfNullOrWhiteSpace(profileCompletionToken, nameof(profileCompletionToken));
      user = await FindUserAsync(profileCompletionToken, cancellationToken);
    }

    OneTimePassword oneTimePassword = await _oneTimePasswordService.ValidateAsync(payload, ContactVerificationPurpose, cancellationToken);
    Phone phone = oneTimePassword.GetPhone();

    if (profileCompletionToken == null)
    {
      user = await _userService.UpdatePhoneAsync(user.Id, phone, cancellationToken);
      return new ChangePhoneResult(user.ToUserProfile());
    }

    List<TokenClaim> claims = new(capacity: 3)
    {
      new(Rfc7519ClaimNames.PhoneNumber, phone.Number),
      new(Rfc7519ClaimNames.IsPhoneVerified, true.ToString().ToLower(), ClaimValueTypes.Boolean)
    };
    if (phone.CountryCode != null)
    {
      claims.Add(new(Claims.PhoneCountryCode, phone.CountryCode));
    }
    CreatedToken createdToken = await _tokenService.CreateAsync(user.GetSubject(), claims, TokenTypes.Profile, cancellationToken);
    return new ChangePhoneResult(createdToken);
  }

  private async Task<User> FindUserAsync(string profileCompletionToken, CancellationToken cancellationToken)
  {
    ValidatedToken validatedToken = await _tokenService.ValidateAsync(profileCompletionToken, consume: false, TokenTypes.Profile, cancellationToken);
    if (validatedToken.Subject == null)
    {
      throw new InvalidOperationException($"The '{validatedToken.Subject}' claim is required.");
    }
    Guid userId = Guid.Parse(validatedToken.Subject);
    return await _userService.FindAsync(userId, cancellationToken) ?? throw new InvalidOperationException($"The user 'Id={userId}' could not be found.");
  }
}
