using Logitar.Portal.Contracts;

namespace Logitar.Master.Contracts.Accounts;

public record UserProfile
{
  public DateTime CreatedOn { get; set; }
  public DateTime CompletedOn { get; set; }
  public DateTime UpdatedOn { get; set; }

  public DateTime? PasswordChangedOn { get; set; }
  public DateTime? AuthenticatedOn { get; set; }
  public MultiFactorAuthenticationMode MultiFactorAuthenticationMode { get; set; }

  public string EmailAddress { get; set; }
  public AccountPhone? Phone { get; set; }

  public string FirstName { get; set; }
  public string? MiddleName { get; set; }
  public string LastName { get; set; }
  public string FullName { get; set; }

  public DateTime? Birthdate { get; set; }
  public string? Gender { get; set; }
  public Locale Locale { get; set; }
  public string TimeZone { get; set; }

  public UserProfile() : this(string.Empty, string.Empty, string.Empty, string.Empty, new Locale(), string.Empty)
  {
  }

  public UserProfile(string emailAddress, string firstName, string lastName, string fullName, Locale locale, string timeZone)
  {
    EmailAddress = emailAddress;

    FirstName = firstName;
    LastName = lastName;
    FullName = fullName;

    Locale = locale;
    TimeZone = timeZone;
  }
}
