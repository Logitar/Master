using Logitar.Master.Contracts.Accounts;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

public record SignInCommand(SignInPayload Payload) : IRequest<SignInCommandResult>;
