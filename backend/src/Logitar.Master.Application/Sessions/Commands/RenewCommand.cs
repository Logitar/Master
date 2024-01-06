using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions.Commands;

internal record RenewCommand(RenewPayload Payload) : IRequest<Session>;
