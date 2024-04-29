using Bogus;
using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

[Trait(Traits.Category, Categories.Unit)]
public class MessageExtensionsTests
{
  private const string Base12Table = "2345679ACDEF";

  private readonly Faker _faker = new();

  [Theory(DisplayName = "GenerateConfirmationNumber: it should generate the correct confirmation number")]
  [InlineData("c52fff6f-c539-471f-95e9-4912a5a0031e", "DR96")]
  public void GenerateConfirmationNumber_it_should_generate_the_correct_confirmation_number(string messageId, string expectedStart)
  {
    SentMessages sentMessages = new([Guid.Parse(messageId)]);
    string[] parts = sentMessages.GenerateConfirmationNumber().Split('-');
    Assert.Equal(3, parts.Length);

    Assert.Equal(4, parts[0].Length);
    Assert.Equal(expectedStart, parts[0]);

    DateTime now = DateTime.UtcNow;
    Assert.Equal(now.ToString("yyMMdd"), parts[1]);

    Assert.Equal(2, parts[2].Length);
    Assert.Equal(Base12Table[(now.Hour * 6 + (now.Minute / 10)) / 12], parts[2][0]);
    Assert.Equal(Base12Table[(now.Hour * 6 + (now.Minute / 10)) % 12], parts[2][1]);
  }

  [Fact(DisplayName = "GenerateConfirmationNumber: it should throw ArgumentException when multiple messages were sent.")]
  public void GenerateConfirmationNumber_it_should_throw_ArgumentException_when_multiple_messages_were_sent()
  {
    SentMessages sentMessages = new([Guid.NewGuid(), Guid.NewGuid()]);
    var exception = Assert.Throws<ArgumentException>(sentMessages.GenerateConfirmationNumber);
    Assert.StartsWith("More than one message have been sent.", exception.Message);
    Assert.Equal("sentMessages", exception.ParamName);
  }

  [Fact(DisplayName = "GenerateConfirmationNumber: it should throw ArgumentException when no message was sent.")]
  public void GenerateConfirmationNumber_it_should_throw_ArgumentException_when_no_message_was_sent()
  {
    SentMessages sentMessages = new([]);
    var exception = Assert.Throws<ArgumentException>(sentMessages.GenerateConfirmationNumber);
    Assert.StartsWith("No message has been sent.", exception.Message);
    Assert.Equal("sentMessages", exception.ParamName);
  }

  [Fact(DisplayName = "ToSentMessage: it should return the correct sent email message.")]
  public void ToSentMessage_it_should_return_the_correct_sent_email_message()
  {
    SentMessages sentMessages = new([Guid.NewGuid()]);
    Email email = new(_faker.Person.Email);
    SentMessage sentMessage = sentMessages.ToSentMessage(email);

    string confirmationNumber = sentMessages.GenerateConfirmationNumber();
    Assert.Equal(confirmationNumber, sentMessage.ConfirmationNumber);
    Assert.Equal(ContactType.Email, sentMessage.ContactType);
    Assert.Equal(email.Address, sentMessage.MaskedContact);
  }

  [Fact(DisplayName = "ToSentMessage: it should return the correct sent phone message.")]
  public void ToSentMessage_it_should_return_the_correct_sent_phone_message()
  {
    SentMessages sentMessages = new([Guid.NewGuid()]);
    Phone phone = new(countryCode: "CA", number: "(514) 845-4636", extension: null, e164Formatted: "+15148454636");
    SentMessage sentMessage = sentMessages.ToSentMessage(phone);

    string confirmationNumber = sentMessages.GenerateConfirmationNumber();
    Assert.Equal(confirmationNumber, sentMessage.ConfirmationNumber);
    Assert.Equal(ContactType.Phone, sentMessage.ContactType);
    Assert.Equal("********636", sentMessage.MaskedContact);
  }
}
