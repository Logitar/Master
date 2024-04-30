using Logitar.Master.Contracts.Accounts;
using MediatR;

namespace Logitar.Master.Application.Accounts.Commands;

public record ChangePhoneCommand(ChangePhonePayload Payload) : Activity, IRequest<ChangePhoneResult>;
