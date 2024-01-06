using Logitar.Master.Contracts.Sessions;
using MediatR;

namespace Logitar.Master.Application.Sessions.Commands;

internal record SignOutCommand(string Id) : IRequest<Session>;
