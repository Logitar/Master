using Logitar.Identity.Contracts.Users;

namespace Logitar.Master.Contracts.Accounts;

public record AccountPhone
{
  public string? CountryCode { get; set; }
  public string Number { get; set; }

  public AccountPhone() : this(string.Empty)
  {
  }

  public AccountPhone(IPhone phone) : this(phone.Number, phone.CountryCode)
  {
  }

  public AccountPhone(string number, string? countryCode = null)
  {
    Number = number;
    CountryCode = countryCode;
  }

  public static AccountPhone? TryCreate(IPhone? phone) => phone == null ? null : new(phone);
}
