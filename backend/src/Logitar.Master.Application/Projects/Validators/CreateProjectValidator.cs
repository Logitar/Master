﻿using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Master.Contracts.Projects;
using Logitar.Master.Domain.Projects;

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
