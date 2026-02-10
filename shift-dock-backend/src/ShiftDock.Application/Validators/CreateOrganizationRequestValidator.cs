using FluentValidation;
using ShiftDock.Application.DTOs.Organizations;

namespace ShiftDock.Application.Validators;

public class CreateOrganizationRequestValidator : AbstractValidator<CreateOrganizationRequest>
{
    public CreateOrganizationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Organization name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.DefaultHourlyRate)
            .GreaterThanOrEqualTo(0).WithMessage("Default hourly rate must be non-negative");

        RuleFor(x => x.DefaultContainerRate)
            .GreaterThanOrEqualTo(0).WithMessage("Default container rate must be non-negative");

        RuleFor(x => x.DefaultBoxRate)
            .GreaterThanOrEqualTo(0).WithMessage("Default box rate must be non-negative");
    }
}
