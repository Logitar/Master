using Logitar.Portal.Contracts.ApiKeys;
using Logitar.Portal.Contracts.Sessions;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Master.Application;

public record ActivityContext(ApiKey? ApiKey, User? User, Session? Session);
