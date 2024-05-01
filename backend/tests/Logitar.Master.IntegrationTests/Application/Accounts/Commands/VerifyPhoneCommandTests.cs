using FluentValidation.Results;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Moq;

namespace Logitar.Master.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class VerifyPhoneCommandTests : IntegrationTests
{
  public VerifyPhoneCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should return a new profile completion token with phone claims and invalidate the old token.")]
  public async Task It_should_return_a_new_profile_completion_token_with_phone_claims_and_invalidate_the_old_token()
  {
    User user = new(Faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("UserId", user.Id.ToString()));
    Phone phone = new("CA", "(514) 845-4636", extension: null, "+15148454636")
    {
      IsVerified = true
    };
    Assert.NotNull(phone.CountryCode);
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("PhoneCountryCode", phone.CountryCode));
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("PhoneNumber", phone.Number));
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("PhoneE164Formatted", phone.E164Formatted));
    OneTimePasswordPayload oneTimePasswordPayload = new(oneTimePassword.Id, "123456");
    OneTimePasswordService.Setup(x => x.ValidateAsync(oneTimePasswordPayload, "ContactVerification", CancellationToken)).ReturnsAsync(oneTimePassword);

    CreatedToken createdToken = new("new_token");
    TokenService.Setup(x => x.CreateAsync(user.GetSubject(), phone, "profile+jwt", CancellationToken)).ReturnsAsync(createdToken);

    VerifyPhonePayload payload = new(Faker.Locale, "profile_completion_token", oneTimePasswordPayload);
    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    TokenService.Setup(x => x.ValidateAsync(payload.ProfileCompletionToken, false, "profile+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    VerifyPhoneCommand command = new(payload);
    VerifyPhoneResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);

    TokenService.Verify(x => x.ValidateAsync(payload.ProfileCompletionToken, true, "profile+jwt", CancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should send a One-Time Password text message.")]
  public async Task It_should_send_a_One_Time_Password_text_message()
  {
    AccountPhone newPhone = new("(514) 845-4636", "CA");
    Phone phone = newPhone.ToPhone();

    User user = new(Faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid(),
      Password = "123456"
    };
    OneTimePasswordService.Setup(x => x.CreateAsync(user, "ContactVerification", phone, CancellationToken)).ReturnsAsync(oneTimePassword);

    VerifyPhonePayload payload = new(Faker.Locale, "profile_completion_token", newPhone);
    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    TokenService.Setup(x => x.ValidateAsync(payload.ProfileCompletionToken, false, "profile+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    MessageService.Setup(x => x.SendAsync("ContactVerificationPhone", phone,
      payload.Locale, It.Is<Dictionary<string, string>>(v => v.Count == 1 && v["OneTimePassword"] == oneTimePassword.Password), CancellationToken)
    ).ReturnsAsync(sentMessages);

    VerifyPhoneCommand command = new(payload);
    VerifyPhoneResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.OneTimePasswordValidation);
    Assert.Equal(oneTimePassword.Id, result.OneTimePasswordValidation.OneTimePasswordId);
    Assert.Equal(sentMessages.ToSentMessage(phone), result.OneTimePasswordValidation.SentMessage);
  }

  [Fact(DisplayName = "It should throw InvalidOneTimePasswordUserException when the One-Time Password was meant for another user.")]
  public async Task It_should_throw_InvalidOneTimePasswordUserException_when_the_One_Time_Password_was_meant_for_another_user()
  {
    User user = new(Faker.Person.UserName)
    {
      Id = Guid.NewGuid()
    };
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("UserId", Guid.Empty.ToString()));
    OneTimePasswordPayload oneTimePasswordPayload = new(oneTimePassword.Id, "123456");
    OneTimePasswordService.Setup(x => x.ValidateAsync(oneTimePasswordPayload, "ContactVerification", CancellationToken)).ReturnsAsync(oneTimePassword);

    VerifyPhonePayload payload = new(Faker.Locale, "profile_completion_token", oneTimePasswordPayload);
    ValidatedToken validatedToken = new()
    {
      Subject = user.GetSubject()
    };
    TokenService.Setup(x => x.ValidateAsync(payload.ProfileCompletionToken, false, "profile+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    VerifyPhoneCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidOneTimePasswordUserException>(async () => await Pipeline.ExecuteAsync(command, CancellationToken));
    Assert.Equal(oneTimePassword.Id, exception.OneTimePasswordId);
    Assert.Equal(Guid.Empty, exception.ExpectedUserId);
    Assert.Equal(user.Id, exception.ActualUserId);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    VerifyPhonePayload payload = new(Faker.Locale, "profile_completion_token");
    VerifyPhoneCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("VerifyPhoneValidator", error.ErrorCode);
    Assert.Equal("Exactly one of the following must be specified: NewPhone, OneTimePassword.", error.ErrorMessage);
  }
}
