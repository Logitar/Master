using Logitar.Master.Contracts.Projects;
using MediatR;

namespace Logitar.Master.Application.Projects.Queries;

public record ReadProjectQuery(Guid? Id, string? UniqueKey) : IRequest<Project?>;
