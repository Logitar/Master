using FluentValidation;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;

namespace Logitar.Master.Application.Projects.Validators;

internal class CreateProjectPayloadValidator : AbstractValidator<CreateProjectPayload>
{
  public CreateProjectPayloadValidator()
  {
    RuleFor(x => x.UniqueKey).SetValidator(new UniqueKeyValidator());
    When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => x.Description != null, () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }
}
