using CollegeManagement.API.DTOs.Sections;
using FluentValidation;

namespace CollegeManagement.API.Validators
{
    public class CreateSectionRequestValidator : AbstractValidator<CreateSectionRequest>
    {
        public CreateSectionRequestValidator()
        {
            RuleFor(x => x.Board)
                .NotEmpty().WithMessage("Board is required.")
                .MaximumLength(100).WithMessage("Board cannot exceed 100 characters.");

            RuleFor(x => x.AcademicYearId)
                .GreaterThan(0).WithMessage("Valid Academic Year ID is required.");

            RuleFor(x => x.Group)
                .NotEmpty().WithMessage("Group is required.")
                .MaximumLength(100).WithMessage("Group cannot exceed 100 characters.");

            RuleFor(x => x.AcademicLevel)
                .NotEmpty().WithMessage("Academic Level is required.")
                .MaximumLength(50).WithMessage("Academic Level cannot exceed 50 characters.");

            RuleFor(x => x.SectionName)
                .NotEmpty().WithMessage("Section Name is required.")
                .MaximumLength(50).WithMessage("Section Name cannot exceed 50 characters.");

            RuleFor(x => x.RoomNumber)
                .MaximumLength(50).WithMessage("Room Number cannot exceed 50 characters.");

            RuleFor(x => x.ClassTeacherId)
                .GreaterThan(0).When(x => x.ClassTeacherId.HasValue).WithMessage("Valid Class Teacher ID is required.");

            RuleFor(x => x.MaximumStrength)
                .GreaterThan(0).WithMessage("Maximum Strength must be greater than 0.")
                .LessThanOrEqualTo(1000).WithMessage("Maximum Strength cannot exceed 1000.");
        }
    }

    public class UpdateSectionRequestValidator : AbstractValidator<UpdateSectionRequest>
    {
        public UpdateSectionRequestValidator()
        {
            RuleFor(x => x.Board)
                .NotEmpty().WithMessage("Board is required.")
                .MaximumLength(100).WithMessage("Board cannot exceed 100 characters.");

            RuleFor(x => x.AcademicYearId)
                .GreaterThan(0).WithMessage("Valid Academic Year ID is required.");

            RuleFor(x => x.Group)
                .NotEmpty().WithMessage("Group is required.")
                .MaximumLength(100).WithMessage("Group cannot exceed 100 characters.");

            RuleFor(x => x.AcademicLevel)
                .NotEmpty().WithMessage("Academic Level is required.")
                .MaximumLength(50).WithMessage("Academic Level cannot exceed 50 characters.");

            RuleFor(x => x.SectionName)
                .NotEmpty().WithMessage("Section Name is required.")
                .MaximumLength(50).WithMessage("Section Name cannot exceed 50 characters.");

            RuleFor(x => x.RoomNumber)
                .MaximumLength(50).WithMessage("Room Number cannot exceed 50 characters.");

            RuleFor(x => x.ClassTeacherId)
                .GreaterThan(0).When(x => x.ClassTeacherId.HasValue).WithMessage("Valid Class Teacher ID is required.");

            RuleFor(x => x.MaximumStrength)
                .GreaterThan(0).WithMessage("Maximum Strength must be greater than 0.")
                .LessThanOrEqualTo(1000).WithMessage("Maximum Strength cannot exceed 1000.");
        }
    }
}
