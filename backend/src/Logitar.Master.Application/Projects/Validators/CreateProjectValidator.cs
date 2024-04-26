using FluentValidation;
using Logitar.Master.Domain.Projects;
using Logitar.Master.Domain.Shared;
using Logitar.Portal.Contracts.Projects;

namespace Logitar.Master.Application.Projects.Validators;

internal class CreateProjectValidator : AbstractValidator<CreateProjectPayload>
{
  public CreateProjectValidator()
  {
    RuleFor(x => x.UniqueKey).SetValidator(new UniqueKeyValidator());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
