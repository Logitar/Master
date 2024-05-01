using Logitar.Master.Contracts.Accounts;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

public record VerifyPhoneCommand(VerifyPhonePayload Payload) : IRequest<VerifyPhoneResult>;
