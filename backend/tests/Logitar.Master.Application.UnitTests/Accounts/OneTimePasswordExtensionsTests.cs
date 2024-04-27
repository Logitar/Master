//using Bogus;
//using Logitar.Portal.Contracts.Passwords;
//using Logitar.Portal.Contracts.Users;

//namespace Logitar.Master.Application.Accounts;

//[Trait(Traits.Category, Categories.Unit)]
//public class OneTimePasswordExtensionsTests
//{
//  private const string Purpose = "MultiFactorAuthentication";

//  private readonly Faker _faker = new();

//  [Fact(DisplayName = "EnsurePurpose: it should do nothing when the One-Time Password has the expected purpose.")]
//  public void EnsurePurpose_it_should_do_nothing_when_the_One_Time_Password_has_the_expected_purpose()
//  {
//    OneTimePassword password = new();
//    password.CustomAttributes.Add(new("Purpose", Purpose));
//    password.EnsurePurpose(Purpose);
//  }

//  [Fact(DisplayName = "EnsurePurpose: it should throw InvalidOneTimePasswordPurpose when the One-Time Password has no purpose.")]
//  public void EnsurePurpose_it_should_throw_InvalidOneTimePasswordPurpose_when_the_One_Time_Password_has_no_purpose()
//  {
//    OneTimePassword password = new()
//    {
//      Id = Guid.NewGuid()
//    };
//    Assert.Empty(password.CustomAttributes);
//    var exception = Assert.Throws<InvalidOneTimePasswordPurpose>(() => password.EnsurePurpose(Purpose));
//    Assert.Equal(password.Id, exception.OneTimePasswordId);
//    Assert.Equal(Purpose, exception.ExpectedPurpose);
//    Assert.Null(exception.ActualPurpose);
//  }

//  [Fact(DisplayName = "EnsurePurpose: it should throw InvalidOneTimePasswordPurpose when the purpose is not expected.")]
//  public void EnsurePurpose_it_should_throw_InvalidOneTimePasswordPurpose_when_the_purpose_is_not_expected()
//  {
//    OneTimePassword password = new()
//    {
//      Id = Guid.NewGuid()
//    };

//    string purpose = "Test";
//    password.CustomAttributes.Add(new("Purpose", purpose));

//    var exception = Assert.Throws<InvalidOneTimePasswordPurpose>(() => password.EnsurePurpose(Purpose));
//    Assert.Equal(password.Id, exception.OneTimePasswordId);
//    Assert.Equal(Purpose, exception.ExpectedPurpose);
//    Assert.Equal(purpose, exception.ActualPurpose);
//  }

//  [Fact(DisplayName = "GetPurpose: it should return the purpose when the One-Time Password has one.")]
//  public void GetPurpose_it_should_return_the_purpose_when_the_One_Time_Password_has_one()
//  {
//    OneTimePassword password = new();
//    password.CustomAttributes.Add(new("Purpose", Purpose));
//    Assert.Equal(Purpose, password.GetPurpose());
//  }

//  [Fact(DisplayName = "GetPurpose: it should throw InvalidOperationException when the One-Time Password has no purpose.")]
//  public void GetPurpose_it_should_throw_InvalidOperationException_when_the_One_Time_Password_has_no_purpose()
//  {
//    OneTimePassword password = new();
//    Assert.Empty(password.CustomAttributes);
//    var exception = Assert.Throws<InvalidOperationException>(password.GetPurpose);
//    Assert.Equal("The One-Time Password (OTP) has no 'Purpose' custom attribute.", exception.Message);
//  }

//  [Fact(DisplayName = "GetUserId: it should return the correct identifier.")]
//  public void GetUserId_it_should_return_the_correct_identifier()
//  {
//    Guid userId = Guid.NewGuid();
//    OneTimePassword password = new();
//    password.CustomAttributes.Add(new("UserId", userId.ToString()));
//    Assert.Equal(userId, password.GetUserId());
//  }

//  [Fact(DisplayName = "GetUserId: it should throw InvalidOperationException when the One-Time Password does not have the custom attribute.")]
//  public void GetUserId_it_should_throw_InvalidOperationException_when_the_One_Time_Password_does_not_have_the_custom_attribute()
//  {
//    Guid userId = Guid.NewGuid();
//    OneTimePassword password = new();
//    Assert.Empty(password.CustomAttributes);

//    var exception = Assert.Throws<InvalidOperationException>(() => password.GetUserId());
//    Assert.Equal("The One-Time Password (OTP) has no 'UserId' custom attribute.", exception.Message);
//  }

//  [Fact(DisplayName = "HasPurpose: it should return false when the One-Time Password does not have the expected purpose.")]
//  public void HasPurpose_it_should_return_false_when_the_One_Time_Password_does_not_have_the_expected_purpose()
//  {
//    OneTimePassword password = new();
//    Assert.Empty(password.CustomAttributes);
//    Assert.False(password.HasPurpose(Purpose));

//    password.CustomAttributes.Add(new("Purpose", "Test"));
//    Assert.False(password.HasPurpose(Purpose));
//  }

//  [Fact(DisplayName = "HasPurpose: it should return true when the One-Time Password has the expected purpose.")]
//  public void HasPurpose_it_should_return_true_when_the_One_Time_Password_has_the_expected_purpose()
//  {
//    OneTimePassword password = new();
//    password.CustomAttributes.Add(new("Purpose", Purpose.ToUpper()));
//    Assert.True(password.HasPurpose(Purpose.ToLower()));
//  }

//  [Fact(DisplayName = "SetPurpose: it should add the correct custom attribute.")]
//  public void SetPurpose_it_should_add_the_correct_custom_attribute()
//  {
//    CreateOneTimePasswordPayload payload = new();
//    Assert.Empty(payload.CustomAttributes);

//    payload.SetPurpose(Purpose);
//    Assert.Contains(payload.CustomAttributes, c => c.Key == "Purpose" && c.Value == Purpose);
//  }

//  [Fact(DisplayName = "SetPurpose: it should replace the correct custom attribute.")]
//  public void SetPurpose_it_should_replace_the_correct_custom_attribute()
//  {
//    CreateOneTimePasswordPayload payload = new();
//    payload.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));

//    payload.SetPurpose(Purpose);
//    Assert.Single(payload.CustomAttributes, c => c.Key == "Purpose" && c.Value == Purpose);
//  }

//  [Fact(DisplayName = "SetUserId: it should add the correct custom attribute.")]
//  public void SetUserId_it_should_add_the_correct_custom_attribute()
//  {
//    CreateOneTimePasswordPayload payload = new();
//    Assert.Empty(payload.CustomAttributes);

//    User user = new(_faker.Person.UserName)
//    {
//      Id = Guid.NewGuid()
//    };
//    payload.SetUserId(user);
//    Assert.Contains(payload.CustomAttributes, c => c.Key == "UserId" && c.Value == user.Id.ToString());
//  }

//  [Fact(DisplayName = "SetUserId: it should replace the correct custom attribute.")]
//  public void SetUserId_it_should_replace_the_correct_custom_attribute()
//  {
//    CreateOneTimePasswordPayload payload = new();
//    payload.CustomAttributes.Add(new("UserId", Guid.NewGuid().ToString()));

//    User user = new(_faker.Person.UserName)
//    {
//      Id = Guid.NewGuid()
//    };
//    payload.SetUserId(user);
//    Assert.Single(payload.CustomAttributes, c => c.Key == "UserId" && c.Value == user.Id.ToString());
//  }

//  [Fact(DisplayName = "TryGetPurpose: it should return null when the One-Time Password has no purpose.")]
//  public void TryGetPurpose_it_should_return_null_when_the_One_Time_Password_has_no_purpose()
//  {
//    OneTimePassword password = new();
//    Assert.Empty(password.CustomAttributes);
//    Assert.Null(password.TryGetPurpose());
//  }

//  [Fact(DisplayName = "TryGetPurpose: it should return the purpose when the One-Time Password has one.")]
//  public void TryGetPurpose_it_should_return_the_purpose_when_the_One_Time_Password_has_one()
//  {
//    OneTimePassword password = new();
//    password.CustomAttributes.Add(new("Purpose", Purpose));
//    Assert.Equal(Purpose, password.TryGetPurpose());
//  }
//}
