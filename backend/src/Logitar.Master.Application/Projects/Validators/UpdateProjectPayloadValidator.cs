using FluentValidation;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;

namespace Logitar.Master.Application.Projects.Validators;

internal class UpdateProjectPayloadValidator : AbstractValidator<UpdateProjectPayload>
{
  public UpdateProjectPayloadValidator()
  {
    When(x => x.UniqueKey != null, () => RuleFor(x => x.UniqueKey!).SetValidator(new UniqueKeyValidator()));
    When(x => x.DisplayName?.Value != null, () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => x.Description?.Value != null, () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
