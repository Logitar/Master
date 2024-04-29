namespace Logitar.Master.Contracts.Accounts;

public record SentMessage
{
  public string ConfirmationNumber { get; set; }
  public ContactType ContactType { get; set; }
  public string MaskedContact { get; set; }

  public SentMessage() : this(string.Empty, string.Empty)
  {
  }

  public SentMessage(string confirmationNumber, string maskedContact) : this(confirmationNumber, default, maskedContact)
  {
  }

  public SentMessage(string confirmationNumber, ContactType contactType, string maskedContact)
  {
    ConfirmationNumber = confirmationNumber;
    ContactType = contactType;
    MaskedContact = maskedContact;
  }
}
