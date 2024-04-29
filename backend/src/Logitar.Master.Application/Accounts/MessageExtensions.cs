using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application.Accounts;

internal static class MessageExtensions
{
  private const string Base12Table = "2345679ACDEF";
  private const string Base32Table = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

  public static string GenerateConfirmationNumber(this SentMessages sentMessages)
  {
    if (sentMessages.Ids.Count == 0)
    {
      throw new ArgumentException("No message has been sent.", nameof(sentMessages));
    }
    else if (sentMessages.Ids.Count > 1)
    {
      throw new ArgumentException("More than one message have been sent.", nameof(sentMessages));
    }

    StringBuilder number = new();

    string id = Convert.ToBase64String(sentMessages.Ids.Single().ToByteArray());
    int total = 0;
    for (int i = 0; i <= 2; i++)
    {
      total += (int)Math.Pow(64, 2 - i) * GetBase64Value(id[i]);
    }
    for (int i = 3; i >= 0; i--)
    {
      int divider = (int)Math.Pow(32, i);
      int value = total / divider;
      total %= divider;
      number.Append(GetBase32Character(value));
    }
    DateTime now = DateTime.UtcNow;
    number.Append('-').Append((now.Year % 100).ToString("D2")).Append(now.Month.ToString("D2")).Append(now.Day.ToString("D2")).Append('-');

    int minuteRange = ((now.Hour * 60) + now.Minute) / 10;
    number.Append(GetBase12Character(minuteRange / 12));
    number.Append(GetBase12Character(minuteRange % 12));

    return number.ToString();
  }
  private static char GetBase12Character(int value) => Base12Table[value];
  private static char GetBase32Character(int value) => Base32Table[value];
  private static int GetBase64Value(char c)
  {
    if (c >= 'A' && c <= 'Z')
    {
      return c - 'A';
    }
    else if (c >= 'a' && c <= 'z')
    {
      return c - 'a' + 26;
    }
    else if (c >= '0' && c <= '9')
    {
      return c - '0' + 52;
    }
    else if (c == '+')
    {
      return 62;
    }
    return 63;
  }

  public static SentMessage ToSentMessage(this SentMessages sentMessages, Contact contact)
  {
    if (contact is Email email)
    {
      return new SentMessage(sentMessages.GenerateConfirmationNumber(), ContactType.Email, email.Address);
    }
    else if (contact is Phone phone)
    {
      int length = phone.E164Formatted.Length;
      string maskedContact = string.Concat(phone.E164Formatted.Substring(1, length - 4).Mask(), phone.E164Formatted[(length - 3)..]);
      return new SentMessage(sentMessages.GenerateConfirmationNumber(), ContactType.Phone, maskedContact);
    }

    throw new ArgumentException($"The contact type '{contact.GetType().GetNamespaceQualifiedName()}' is not supported.", nameof(contact));
  }
}
