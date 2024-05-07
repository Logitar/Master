using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Moq;

namespace Logitar.Master.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ResetPasswordCommandTests : IntegrationTests
{
  public ResetPasswordCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should fake an email sending when the user is not found.")]
  public async Task It_should_fake_an_email_sending_when_the_user_is_not_found()
  {
    ResetPasswordPayload payload = new("fr", Faker.Person.Email);
    ResetPasswordCommand command = new(payload, CustomAttributes: []);
    ResetPasswordResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.RecoveryLinkSentTo);
    Assert.NotEqual(string.Empty, result.RecoveryLinkSentTo.ConfirmationNumber);
    Assert.Equal(ContactType.Email, result.RecoveryLinkSentTo.ContactType);
    Assert.Equal(payload.EmailAddress, result.RecoveryLinkSentTo.MaskedContact);
  }

  [Fact(DisplayName = "It should issue a new session.")]
  public async Task It_should_issue_a_new_session()
  {
    ResetPasswordPayload payload = new(Faker.Locale, "password_token", "Test123!");
    Assert.NotNull(payload.Token);
    Assert.NotNull(payload.NewPassword);

    User user = new(Faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    user.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.Email.ToString()));
    user.CustomAttributes.Add(new CustomAttribute("ProfileCompletedOn", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)));
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);
    UserService.Setup(x => x.ResetPasswordAsync(user.Id, payload.NewPassword, CancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    TokenService.Setup(x => x.ValidateAsync(payload.Token, "reset_password+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    CustomAttribute[] customAttributes = [new("IpAddress", Faker.Internet.Ip())];
    Session session = new(user)
    {
      Id = Guid.NewGuid()
    };
    SessionService.Setup(x => x.CreateAsync(user, customAttributes, CancellationToken)).ReturnsAsync(session);

    ResetPasswordCommand command = new(payload, customAttributes);
    ResetPasswordResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Same(session, result.Session);

    UserService.Verify(x => x.ResetPasswordAsync(user.Id, payload.NewPassword, CancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should require to complete the user profile.")]
  public async Task It_should_require_to_complete_the_user_profile()
  {
    ResetPasswordPayload payload = new(Faker.Locale, "password_token", "Test123!");
    Assert.NotNull(payload.Token);
    Assert.NotNull(payload.NewPassword);

    User user = new(Faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);
    UserService.Setup(x => x.ResetPasswordAsync(user.Id, payload.NewPassword, CancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    TokenService.Setup(x => x.ValidateAsync(payload.Token, "reset_password+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    CreatedToken createdToken = new("profile_token");
    TokenService.Setup(x => x.CreateAsync(user.Id.ToString(), "profile+jwt", CancellationToken)).ReturnsAsync(createdToken);

    ResetPasswordCommand command = new(payload, CustomAttributes: []);
    ResetPasswordResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);

    UserService.Verify(x => x.ResetPasswordAsync(user.Id, payload.NewPassword, CancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should send an email to the found user.")]
  public async Task It_should_send_an_email_to_the_found_user()
  {
    User user = new(Faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    UserService.Setup(x => x.FindAsync(Faker.Person.Email, CancellationToken)).ReturnsAsync(user);

    CreatedToken createdToken = new("token");
    TokenService.Setup(x => x.CreateAsync(user.GetSubject(), "reset_password+jwt", CancellationToken)).ReturnsAsync(createdToken);

    ResetPasswordPayload payload = new("fr", Faker.Person.Email);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    MessageService.Setup(x => x.SendAsync("PasswordRecovery", user, ContactType.Email, payload.Locale,
      It.Is<Dictionary<string, string>>(v => v.Count == 1 && v["Token"] == createdToken.Token), CancellationToken
    )).ReturnsAsync(sentMessages);

    ResetPasswordCommand command = new(payload, CustomAttributes: []);
    ResetPasswordResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.RecoveryLinkSentTo);
    Assert.Equal(sentMessages.GenerateConfirmationNumber(), result.RecoveryLinkSentTo.ConfirmationNumber);
    Assert.Equal(ContactType.Email, result.RecoveryLinkSentTo.ContactType);
    Assert.Equal(payload.EmailAddress, result.RecoveryLinkSentTo.MaskedContact);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ResetPasswordPayload payload = new("fr");
    ResetPasswordCommand command = new(payload, CustomAttributes: []);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command, CancellationToken));

    Assert.Equal(2, exception.Errors.Count());
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotEmptyValidator" && e.PropertyName == "Token");
    Assert.Contains(exception.Errors, e => e.ErrorCode == "NotNullValidator" && e.PropertyName == "NewPassword");
  }
}
