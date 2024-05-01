using FluentValidation.Results;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Users;
using Moq;

namespace Logitar.Master.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class ChangePhoneCommandTests : IntegrationTests
{
  public ChangePhoneCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should send a One-Time Password text message.")]
  public async Task It_should_send_a_One_Time_Password_text_message()
  {
    AccountPhone newPhone = new("(514) 845-4636", "CA");
    Phone phone = newPhone.ToPhone();

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid(),
      Password = "123456"
    };
    OneTimePasswordService.Setup(x => x.CreateAsync(It.Is<User>(u => u.Id == Actor.Id), "ContactVerification", phone, CancellationToken))
      .ReturnsAsync(oneTimePassword);

    ChangePhonePayload payload = new(Faker.Locale, newPhone);
    SentMessages sentMessages = new([Guid.NewGuid()]);
    MessageService.Setup(x => x.SendAsync("ContactVerificationPhone", phone,
      payload.Locale, It.Is<Dictionary<string, string>>(v => v.Count == 1 && v["OneTimePassword"] == oneTimePassword.Password), CancellationToken)
    ).ReturnsAsync(sentMessages);

    ChangePhoneCommand command = new(payload);
    ChangePhoneResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.OneTimePasswordValidation);
    Assert.Equal(oneTimePassword.Id, result.OneTimePasswordValidation.OneTimePasswordId);
    Assert.Equal(sentMessages.ToSentMessage(phone), result.OneTimePasswordValidation.SentMessage);
  }

  [Fact(DisplayName = "It should throw InvalidOneTimePasswordUserException when the One-Time Password was meant for another user.")]
  public async Task It_should_throw_InvalidOneTimePasswordUserException_when_the_One_Time_Password_was_meant_for_another_user()
  {
    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("UserId", Guid.Empty.ToString()));
    OneTimePasswordPayload oneTimePasswordPayload = new(oneTimePassword.Id, "123456");
    OneTimePasswordService.Setup(x => x.ValidateAsync(oneTimePasswordPayload, "ContactVerification", CancellationToken)).ReturnsAsync(oneTimePassword);

    ChangePhonePayload payload = new(Faker.Locale, oneTimePasswordPayload);
    ChangePhoneCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<InvalidOneTimePasswordUserException>(async () => await Pipeline.ExecuteAsync(command, CancellationToken));
    Assert.Equal(oneTimePassword.Id, exception.OneTimePasswordId);
    Assert.Equal(Guid.Empty, exception.ExpectedUserId);
    Assert.Equal(Actor.Id, exception.ActualUserId);
  }

  [Fact(DisplayName = "It should throw ValidationException when the payload is not valid.")]
  public async Task It_should_throw_ValidationException_when_the_payload_is_not_valid()
  {
    ChangePhonePayload payload = new(Faker.Locale);
    ChangePhoneCommand command = new(payload);
    var exception = await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await Pipeline.ExecuteAsync(command));

    ValidationFailure error = Assert.Single(exception.Errors);
    Assert.Equal("ChangePhoneValidator", error.ErrorCode);
    Assert.Equal("Exactly one of the following must be specified: NewPhone, OneTimePassword.", error.ErrorMessage);
  }

  [Fact(DisplayName = "It should update the phone of the user.")]
  public async Task It_should_update_the_phone_of_the_user()
  {
    User user = new(Faker.Person.Email)
    {
      Id = Actor.Id,
      Phone = new Phone("CA", "(514) 845-4636", extension: null, "+15148454636")
      {
        IsVerified = true
      }
    };
    UserService.Setup(x => x.UpdatePhoneAsync(Actor.Id, user.Phone, CancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid()
    };
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("UserId", Actor.Id.ToString()));
    Assert.NotNull(user.Phone.CountryCode);
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("PhoneCountryCode", user.Phone.CountryCode));
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("PhoneNumber", user.Phone.Number));
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("PhoneE164Formatted", user.Phone.E164Formatted));
    OneTimePasswordPayload oneTimePasswordPayload = new(oneTimePassword.Id, "123456");
    OneTimePasswordService.Setup(x => x.ValidateAsync(oneTimePasswordPayload, "ContactVerification", CancellationToken)).ReturnsAsync(oneTimePassword);

    ChangePhonePayload payload = new(Faker.Locale, oneTimePasswordPayload);
    ChangePhoneCommand command = new(payload);
    ChangePhoneResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.UserProfile);
    Assert.NotNull(result.UserProfile.Phone);
    Assert.Equal(user.Phone.CountryCode, result.UserProfile.Phone.CountryCode);
    Assert.Equal(user.Phone.Number, result.UserProfile.Phone.Number);
  }
}
