using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Queries;

internal record ReadProjectQuery(string? Id, string? UniqueKey) : IRequest<Project?>;
