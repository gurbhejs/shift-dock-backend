using FluentValidation;
using ShiftDock.Application.DTOs.Projects;

namespace ShiftDock.Application.Validators;

public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
{
    public CreateProjectRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Project name is required")
            .MaximumLength(200).WithMessage("Project name must not exceed 200 characters");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required")
            .MaximumLength(500).WithMessage("Location must not exceed 500 characters");

        RuleFor(x => x.Latitude)
            .MaximumLength(50).WithMessage("Latitude must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Latitude));

        RuleFor(x => x.Longitude)
            .MaximumLength(50).WithMessage("Longitude must not exceed 50 characters")
            .When(x => !string.IsNullOrEmpty(x.Longitude));

        RuleFor(x => x.Notes)
            .MaximumLength(2000).WithMessage("Notes must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.WorkType)
            .IsInEnum().WithMessage("Invalid work type");

        RuleFor(x => x.Rate)
            .GreaterThanOrEqualTo(0).WithMessage("Rate must be a positive value");
    }
}
