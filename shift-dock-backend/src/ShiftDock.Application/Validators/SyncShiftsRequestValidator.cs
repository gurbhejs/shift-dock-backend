using FluentValidation;
using ShiftDock.Application.DTOs.Projects;

namespace ShiftDock.Application.Validators;

public class SyncShiftsRequestValidator : AbstractValidator<SyncShiftsRequest>
{
    public SyncShiftsRequestValidator()
    {
        RuleFor(x => x.Shifts)
            .NotNull()
            .WithMessage("Shifts list is required");

        RuleForEach(x => x.Shifts).ChildRules(shift =>
        {
            shift.RuleFor(s => s.ShiftDate)
                .NotEmpty()
                .WithMessage("Shift date is required")
                .Matches(@"^\d{4}-\d{2}-\d{2}$")
                .WithMessage("Shift date must be in YYYY-MM-DD format");

            shift.RuleFor(s => s.StartTime)
                .NotEmpty()
                .WithMessage("Start time is required")
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage("Start time must be in HH:mm format");

            shift.RuleFor(s => s.EndTime)
                .NotEmpty()
                .WithMessage("End time is required")
                .Matches(@"^\d{2}:\d{2}$")
                .WithMessage("End time must be in HH:mm format");

            shift.RuleFor(s => s.Rate)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Rate must be greater than or equal to 0");

            shift.RuleFor(s => s.TargetQuantity)
                .GreaterThan(0)
                .When(s => s.TargetQuantity.HasValue)
                .WithMessage("Target quantity must be greater than 0 when specified");
        });
    }
}
