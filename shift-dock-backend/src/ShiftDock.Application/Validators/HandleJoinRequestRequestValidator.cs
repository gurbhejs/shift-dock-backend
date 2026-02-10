using FluentValidation;
using ShiftDock.Application.DTOs.Organizations;

namespace ShiftDock.Application.Validators;

public class HandleJoinRequestRequestValidator : AbstractValidator<HandleJoinRequestRequest>
{
    public HandleJoinRequestRequestValidator()
    {
        RuleFor(x => x.RequestIds)
            .NotNull()
            .WithMessage("Request IDs list is required")
            .NotEmpty()
            .WithMessage("At least one request ID must be provided");

        RuleForEach(x => x.RequestIds)
            .NotEmpty()
            .WithMessage("Request ID cannot be empty");
    }
}
