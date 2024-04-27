using Logitar.Master.Contracts.Accounts;
using Logitar.Portal.Contracts;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

public record SignInCommand(SignInPayload Payload, IEnumerable<CustomAttribute> CustomAttributes) : IRequest<SignInCommandResult>;
