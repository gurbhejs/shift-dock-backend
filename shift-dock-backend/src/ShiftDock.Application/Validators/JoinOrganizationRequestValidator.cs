using FluentValidation;
using ShiftDock.Application.DTOs.Organizations;

namespace ShiftDock.Application.Validators;

public class JoinOrganizationRequestValidator : AbstractValidator<JoinOrganizationRequest>
{
    public JoinOrganizationRequestValidator()
    {
        RuleFor(x => x.OrganizationCode)
            .NotEmpty().WithMessage("Organization code is required")
            .Length(4, 20).WithMessage("Code must be between 4 and 20 characters");
    }
}
