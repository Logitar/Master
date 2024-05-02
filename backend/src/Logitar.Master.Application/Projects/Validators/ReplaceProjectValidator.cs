using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts.Projects;

namespace Logitar.Master.Application.Projects.Validators;

internal class ReplaceProjectValidator : AbstractValidator<ReplaceProjectPayload>
{
  public ReplaceProjectValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
