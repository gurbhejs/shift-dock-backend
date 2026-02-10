using FluentValidation;
using ShiftDock.Application.DTOs.Organizations;

namespace ShiftDock.Application.Validators;

public class UpdateMemberStatusRequestValidator : AbstractValidator<UpdateMemberStatusRequest>
{
    public UpdateMemberStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .WithMessage("Status is required")
            .Must(status => status.Equals("Active", StringComparison.OrdinalIgnoreCase) || 
                           status.Equals("Inactive", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Status must be either 'Active' or 'Inactive'");
    }
}
