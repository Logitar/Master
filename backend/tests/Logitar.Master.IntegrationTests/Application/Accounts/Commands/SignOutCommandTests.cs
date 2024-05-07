using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;
using Moq;

namespace Logitar.Master.Application.Accounts.Commands;

[Trait(Traits.Category, Categories.Integration)]
public class SignOutCommandTests : IntegrationTests
{
  public SignOutCommandTests() : base()
  {
  }

  [Fact(DisplayName = "It should sign out a session when a session ID is provided.")]
  public async Task It_should_sign_out_a_session_when_a_session_Id_is_provided()
  {
    User user = new(Faker.Person.UserName)
    {
      Id = ActorId.ToGuid()
    };
    Session session = new(user)
    {
      Id = Guid.NewGuid()
    };

    SignOutCommand command = new(session);
    await Pipeline.ExecuteAsync(command, CancellationToken);

    SessionService.Verify(x => x.SignOutAsync(session.Id, CancellationToken), Times.Once);
    UserService.Verify(x => x.SignOutAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
  }

  [Fact(DisplayName = "It should sign out an user when an user ID is provided.")]
  public async Task It_should_sign_out_an_user_when_an_user_Id_is_provided()
  {
    User user = new(Faker.Person.UserName)
    {
      Id = ActorId.ToGuid()
    };

    SignOutCommand command = new(user);
    await Pipeline.ExecuteAsync(command, CancellationToken);

    SessionService.Verify(x => x.SignOutAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    UserService.Verify(x => x.SignOutAsync(user.Id, CancellationToken), Times.Once);
  }
}
