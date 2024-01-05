using Logitar.Master.Contracts.Account;
using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Account.Commands;

internal record SignInCommand(SignInPayload Payload) : IRequest<Session>;
