using Logitar.Master.Application.Constants;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Passwords;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Contracts.Users;
using Logitar.Security.Claims;
using Moq;

namespace Logitar.Master.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignInCommandTests : IntegrationTests
{
  public SignInCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should complete the user profile.")]
  public async Task It_should_complete_the_user_profile()
  {
    SignInPayload payload = new(Faker.Locale)
    {
      Profile = new CompleteProfilePayload("profile_token", Faker.Person.FirstName, Faker.Person.LastName, "fr-CA", "America/Montreal")
      {
        Password = "P@s$W0rD",
        MultiFactorAuthenticationMode = MultiFactorAuthenticationMode.Phone,
        Birthdate = Faker.Person.DateOfBirth,
        Gender = Faker.Person.Gender.ToString().ToLower()
      }
    };

    User user = new(Faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);

    User updatedUser = new(user.UniqueName)
    {
      HasPassword = true,
      Email = user.Email,
      Phone = new Phone(countryCode: "CA", "(514) 845-4636", extension: null, e164Formatted: "+15148454636")
      {
        IsVerified = true
      },
      FirstName = Faker.Person.FirstName,
      LastName = Faker.Person.LastName,
      FullName = Faker.Person.FullName,
      Birthdate = Faker.Person.DateOfBirth,
      Gender = Faker.Person.Gender.ToString().ToLower(),
      Locale = new Locale("fr-CA"),
      TimeZone = "America/Montreal"
    };
    updatedUser.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.Phone.ToString()));
    updatedUser.CustomAttributes.Add(new CustomAttribute("ProfileCompletedOn", DateTime.Now.ToString("O", CultureInfo.InvariantCulture)));
    UserService.Setup(x => x.CompleteProfileAsync(user.Id, payload.Profile, updatedUser.Phone, CancellationToken)).ReturnsAsync(updatedUser);

    ValidatedToken validatedToken = new()
    {
      Subject = user.Id.ToString()
    };
    Assert.NotNull(updatedUser.Phone.CountryCode);
    validatedToken.Claims.Add(new TokenClaim(ClaimNames.PhoneCountryCode, updatedUser.Phone.CountryCode));
    validatedToken.Claims.Add(new TokenClaim(ClaimNames.PhoneNumberRaw, updatedUser.Phone.Number));
    validatedToken.Claims.Add(new TokenClaim(Rfc7519ClaimNames.PhoneNumber, updatedUser.Phone.E164Formatted));
    validatedToken.Claims.Add(new TokenClaim(Rfc7519ClaimNames.IsPhoneVerified, "true"));
    TokenService.Setup(x => x.ValidateAsync(payload.Profile.Token, "profile+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    Session session = new(updatedUser);
    CustomAttribute[] customAttributes = [new("IpAddress", Faker.Internet.Ip())];
    SessionService.Setup(x => x.CreateAsync(updatedUser, customAttributes, CancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Same(session, result.Session);
  }

  [Fact(DisplayName = "It should create a new user.")]
  public async Task It_should_create_a_new_user()
  {
    SignInPayload payload = new(Faker.Locale)
    {
      AuthenticationToken = "authentication_token"
    };

    ValidatedToken validatedToken = new()
    {
      Email = new Email(Faker.Person.Email)
    };
    TokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, "auth+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    User user = new(Faker.Person.Email)
    {
      Email = new(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    UserService.Setup(x => x.CreateAsync(It.Is<Email>(e => e.Address == validatedToken.Email.Address && e.IsVerified), CancellationToken)).ReturnsAsync(user);

    CreatedToken createdToken = new("profile_token");
    TokenService.Setup(x => x.CreateAsync(user.Id.ToString(), "profile+jwt", CancellationToken)).ReturnsAsync(createdToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);
  }

  [Fact(DisplayName = "It should require the password when the user has one.")]
  public async Task It_should_require_the_password_when_the_user_has_one()
  {
    User user = new(Faker.Person.Email)
    {
      Email = new Email(Faker.Person.Email),
      HasPassword = true
    };
    UserService.Setup(x => x.FindAsync(user.UniqueName, CancellationToken)).ReturnsAsync(user);

    SignInPayload payload = new(Faker.Locale)
    {
      Credentials = new Credentials(user.UniqueName)
    };
    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.True(result.IsPasswordRequired);
  }

  [Fact(DisplayName = "It should require to complete the user profile (Credentials).")]
  public async Task It_should_require_to_complete_the_user_profile_Credentials()
  {
    User user = new(Faker.Person.Email)
    {
      Email = new Email(Faker.Person.Email),
      HasPassword = true
    };
    user.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.None.ToString()));
    UserService.Setup(x => x.FindAsync(user.UniqueName, CancellationToken)).ReturnsAsync(user);

    CreatedToken token = new("token");
    TokenService.Setup(x => x.CreateAsync(user.Id.ToString(), "profile+jwt", CancellationToken)).ReturnsAsync(token);

    SignInPayload payload = new(Faker.Locale)
    {
      Credentials = new Credentials(user.UniqueName, "P@s$W0rD")
    };
    Assert.NotNull(payload.Credentials.Password);
    UserService.Setup(x => x.AuthenticateAsync(user, payload.Credentials.Password, CancellationToken)).ReturnsAsync(user);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Equal(token.Token, result.ProfileCompletionToken);
  }

  [Fact(DisplayName = "It should require to complete the user profile (Multi-Factor Authentication).")]
  public async Task It_should_require_to_complete_the_user_profile_Multi_Factor_Authentication()
  {
    User user = new(Faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid(),
      Password = "123456"
    };
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("Purpose", "MultiFactorAuthentication"));
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("UserId", user.Id.ToString()));
    SignInPayload payload = new(Faker.Locale)
    {
      OneTimePassword = new OneTimePasswordPayload(oneTimePassword.Id, oneTimePassword.Password)
    };
    OneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, "MultiFactorAuthentication", CancellationToken)).ReturnsAsync(oneTimePassword);

    CreatedToken createdToken = new("profile_token");
    TokenService.Setup(x => x.CreateAsync(user.Id.ToString(), "profile+jwt", CancellationToken)).ReturnsAsync(createdToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);
  }

  [Fact(DisplayName = "It should send a Multi Factor Authentication email message.")]
  public async Task It_should_send_a_Multi_Factor_Authentication_email_message()
  {
    User user = new(Faker.Person.Email)
    {
      Email = new Email(Faker.Person.Email),
      HasPassword = true
    };
    user.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.Email.ToString()));
    UserService.Setup(x => x.FindAsync(user.UniqueName, CancellationToken)).ReturnsAsync(user);

    SignInPayload payload = new(Faker.Locale)
    {
      Credentials = new Credentials(user.UniqueName, "P@s$W0rD")
    };
    Assert.NotNull(payload.Credentials.Password);
    UserService.Setup(x => x.AuthenticateAsync(user, payload.Credentials.Password, CancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid(),
      Password = "123456"
    };
    OneTimePasswordService.Setup(x => x.CreateAsync(user, "MultiFactorAuthentication", CancellationToken)).ReturnsAsync(oneTimePassword);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    MessageService.Setup(x => x.SendAsync("MultiFactorAuthenticationEmail", user, payload.Locale,
      It.Is<Dictionary<string, string>>(v => v.Count == 1 && v["OneTimePassword"] == oneTimePassword.Password), CancellationToken)
    ).ReturnsAsync(sentMessages);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.OneTimePasswordValidation);
    Assert.Equal(oneTimePassword.Id, result.OneTimePasswordValidation.OneTimePasswordId);
    Assert.Equal(sentMessages.ToSentMessage(user.Email), result.OneTimePasswordValidation.SentMessage);
  }

  [Fact(DisplayName = "It should send a Multi Factor Authentication SMS message.")]
  public async Task It_should_send_a_Multi_Factor_Authentication_Sms_message()
  {
    User user = new(Faker.Person.Email)
    {
      Phone = new Phone(countryCode: "CA", number: "(514) 845-4636", extension: null, e164Formatted: "+15148454636"),
      HasPassword = true
    };
    user.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.Phone.ToString()));
    UserService.Setup(x => x.FindAsync(user.UniqueName, CancellationToken)).ReturnsAsync(user);

    SignInPayload payload = new(Faker.Locale)
    {
      Credentials = new Credentials(user.UniqueName, "P@s$W0rD")
    };
    Assert.NotNull(payload.Credentials.Password);
    UserService.Setup(x => x.AuthenticateAsync(user, payload.Credentials.Password, CancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid(),
      Password = "123456"
    };
    OneTimePasswordService.Setup(x => x.CreateAsync(user, "MultiFactorAuthentication", CancellationToken)).ReturnsAsync(oneTimePassword);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    MessageService.Setup(x => x.SendAsync("MultiFactorAuthenticationPhone", user, payload.Locale,
      It.Is<Dictionary<string, string>>(v => v.Count == 1 && v["OneTimePassword"] == oneTimePassword.Password), CancellationToken)
    ).ReturnsAsync(sentMessages);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.OneTimePasswordValidation);
    Assert.Equal(oneTimePassword.Id, result.OneTimePasswordValidation.OneTimePasswordId);
    Assert.Equal(sentMessages.ToSentMessage(user.Phone), result.OneTimePasswordValidation.SentMessage);
  }

  [Fact(DisplayName = "It should send an authentication link when the user does not exist.")]
  public async Task It_should_send_an_authentication_link_when_the_user_does_not_exist()
  {
    SignInPayload payload = new(Faker.Locale)
    {
      Credentials = new Credentials(Faker.Person.Email)
    };

    CreatedToken createdToken = new("token");
    TokenService.Setup(x => x.CreateAsync(null, It.Is<Email>(e => e.Address == payload.Credentials.EmailAddress), "auth+jwt", CancellationToken))
      .ReturnsAsync(createdToken);

    SentMessages sentMessages = new([Guid.NewGuid()]);
    MessageService.Setup(x => x.SendAsync("AccountAuthentication", It.Is<Email>(e => e.Address == payload.Credentials.EmailAddress), payload.Locale,
      It.Is<Dictionary<string, string>>(v => v.Count == 1 && v["Token"] == createdToken.Token), CancellationToken)
    ).ReturnsAsync(sentMessages);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.AuthenticationLinkSentTo);
    Assert.Equal(sentMessages.GenerateConfirmationNumber(), result.AuthenticationLinkSentTo.ConfirmationNumber);
    Assert.Equal(ContactType.Email, result.AuthenticationLinkSentTo.ContactType);
    Assert.Equal(payload.Credentials.EmailAddress, result.AuthenticationLinkSentTo.MaskedContact);

    UserService.Verify(x => x.FindAsync(payload.Credentials.EmailAddress, CancellationToken), Times.Once);
  }

  [Fact(DisplayName = "It should send an authentication link when the user does not have a password.")]
  public async Task It_should_send_an_authentication_link_when_the_user_does_not_have_a_password()
  {
    User user = new(Faker.Person.Email)
    {
      Email = new Email(Faker.Person.Email)
    };
    UserService.Setup(x => x.FindAsync(user.UniqueName, CancellationToken)).ReturnsAsync(user);

    CreatedToken createdToken = new("token");
    TokenService.Setup(x => x.CreateAsync(user.Id.ToString(), user.Email, "auth+jwt", CancellationToken)).ReturnsAsync(createdToken);

    SignInPayload payload = new(Faker.Locale)
    {
      Credentials = new Credentials(user.UniqueName)
    };

    SentMessages sentMessages = new([Guid.NewGuid()]);
    MessageService.Setup(x => x.SendAsync("AccountAuthentication", user, payload.Locale,
      It.Is<Dictionary<string, string>>(v => v.Count == 1 && v["Token"] == createdToken.Token), CancellationToken)
    ).ReturnsAsync(sentMessages);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.NotNull(result.AuthenticationLinkSentTo);
    Assert.Equal(sentMessages.GenerateConfirmationNumber(), result.AuthenticationLinkSentTo.ConfirmationNumber);
    Assert.Equal(ContactType.Email, result.AuthenticationLinkSentTo.ContactType);
    Assert.Equal(payload.Credentials.EmailAddress, result.AuthenticationLinkSentTo.MaskedContact);
  }

  [Fact(DisplayName = "It should sign-in the user when it has no Multi-Factor Authentication and its profile is completed.")]
  public async Task It_should_sign_in_the_user_when_it_has_no_Multi_Factor_Authentication_and_its_profile_is_completed()
  {
    User user = new(Faker.Person.Email)
    {
      Email = new Email(Faker.Person.Email),
      HasPassword = true
    };
    user.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.None.ToString()));
    user.CustomAttributes.Add(new CustomAttribute("ProfileCompletedOn", DateTime.Now.ToString("O", CultureInfo.InvariantCulture)));
    UserService.Setup(x => x.FindAsync(user.UniqueName, CancellationToken)).ReturnsAsync(user);

    SignInPayload payload = new(Faker.Locale)
    {
      Credentials = new Credentials(user.UniqueName, "P@s$W0rD")
    };
    Assert.NotNull(payload.Credentials.Password);
    CustomAttribute[] customAttributes = [new("IpAddress", Faker.Internet.Ip())];

    Session session = new(user);
    SessionService.Setup(x => x.SignInAsync(user, payload.Credentials.Password, customAttributes, CancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Same(session, result.Session);
  }

  [Fact(DisplayName = "It should sign-in the user with an authentication token.")]
  public async Task It_should_sign_in_the_user_with_an_authentication_token()
  {
    SignInPayload payload = new(Faker.Locale)
    {
      AuthenticationToken = "authentication_token"
    };

    User user = new(Faker.Person.Email)
    {
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    user.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.None.ToString()));
    user.CustomAttributes.Add(new CustomAttribute("ProfileCompletedOn", DateTime.Now.ToString("O", CultureInfo.InvariantCulture)));
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);

    ValidatedToken validatedToken = new()
    {
      Subject = user.Id.ToString(),
      Email = user.Email
    };
    TokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, "auth+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    Session session = new(user);
    CustomAttribute[] customAttributes = [new("IpAddress", Faker.Internet.Ip())];
    SessionService.Setup(x => x.CreateAsync(user, customAttributes, CancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Same(session, result.Session);

    UserService.Verify(x => x.UpdateEmailAsync(It.IsAny<Guid>(), It.IsAny<Email>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should sign-in the user with an One-Time Password.")]
  public async Task It_should_sign_in_the_user_with_an_One_Time_Password()
  {
    User user = new(Faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    user.CustomAttributes.Add(new CustomAttribute("MultiFactorAuthenticationMode", MultiFactorAuthenticationMode.Phone.ToString()));
    user.CustomAttributes.Add(new CustomAttribute("ProfileCompletedOn", DateTime.Now.ToString("O", CultureInfo.InvariantCulture)));
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);

    OneTimePassword oneTimePassword = new()
    {
      Id = Guid.NewGuid(),
      Password = "123456"
    };
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("Purpose", "MultiFactorAuthentication"));
    oneTimePassword.CustomAttributes.Add(new CustomAttribute("UserId", user.Id.ToString()));
    SignInPayload payload = new(Faker.Locale)
    {
      OneTimePassword = new OneTimePasswordPayload(oneTimePassword.Id, oneTimePassword.Password)
    };
    OneTimePasswordService.Setup(x => x.ValidateAsync(payload.OneTimePassword, "MultiFactorAuthentication", CancellationToken)).ReturnsAsync(oneTimePassword);

    Session session = new(user);
    CustomAttribute[] customAttributes = [new("IpAddress", Faker.Internet.Ip())];
    SessionService.Setup(x => x.CreateAsync(user, customAttributes, CancellationToken)).ReturnsAsync(session);

    SignInCommand command = new(payload, customAttributes);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Same(session, result.Session);
  }

  [Fact(DisplayName = "It should update the user email and require to complete the user profile (AuthenticationToken).")]
  public async Task It_should_update_the_user_email_and_require_to_complete_the_user_profile_AuthenticationToken()
  {
    User user = new(Faker.Person.Email)
    {
      Id = Guid.NewGuid(),
      Email = new Email(Faker.Person.Email)
    };
    UserService.Setup(x => x.FindAsync(user.Id, CancellationToken)).ReturnsAsync(user);

    SignInPayload payload = new(Faker.Locale)
    {
      AuthenticationToken = "authentication_token"
    };

    ValidatedToken validatedToken = new()
    {
      Subject = user.Id.ToString(),
      Email = user.Email
    };
    TokenService.Setup(x => x.ValidateAsync(payload.AuthenticationToken, "auth+jwt", CancellationToken)).ReturnsAsync(validatedToken);

    User updatedUser = new(user.UniqueName)
    {
      Id = user.Id,
      Email = new Email(Faker.Person.Email)
      {
        IsVerified = true
      }
    };
    UserService.Setup(x => x.UpdateEmailAsync(updatedUser.Id, updatedUser.Email, CancellationToken)).ReturnsAsync(updatedUser);

    CreatedToken createdToken = new("profile_token");
    TokenService.Setup(x => x.CreateAsync(user.Id.ToString(), "profile+jwt", CancellationToken)).ReturnsAsync(createdToken);

    SignInCommand command = new(payload, CustomAttributes: []);
    SignInCommandResult result = await Pipeline.ExecuteAsync(command, CancellationToken);

    Assert.Equal(createdToken.Token, result.ProfileCompletionToken);
  }
}
