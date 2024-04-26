using FluentValidation;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Shared;

namespace Logitar.Master.Application.Projects.Validators;

internal class UpdateProjectValidator : AbstractValidator<UpdateProjectPayload>
{
  public UpdateProjectValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
