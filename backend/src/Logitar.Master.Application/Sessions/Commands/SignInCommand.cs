using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions.Commands;

internal record SignInCommand(SignInPayload Payload) : IRequest<Session>;
