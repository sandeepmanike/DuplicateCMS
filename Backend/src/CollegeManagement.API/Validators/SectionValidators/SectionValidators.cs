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

            RuleFor(x => x.Programme)
                .MaximumLength(100).WithMessage("Programme cannot exceed 100 characters.");

            RuleFor(x => x.SectionName)
                .NotEmpty().WithMessage("Section Name is required.")
                .MaximumLength(50).WithMessage("Section Name cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9 -]+$").WithMessage("Section name may contain only letters, numbers, spaces, and hyphens.");

            RuleFor(x => x.RoomNumber)
                .MaximumLength(50).WithMessage("Room Number cannot exceed 50 characters.");

            RuleFor(x => x.InchargeId)
                .GreaterThan(0).When(x => x.InchargeId.HasValue).WithMessage("Valid Incharge ID is required.");

            RuleFor(x => x.MaximumStrength)
                .GreaterThan(0).WithMessage("Capacity is required and must be greater than 0.")
                .InclusiveBetween(1, 150).WithMessage("Capacity must be between 1 and 150.");
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

            RuleFor(x => x.Programme)
                .MaximumLength(100).WithMessage("Programme cannot exceed 100 characters.");

            RuleFor(x => x.SectionName)
                .NotEmpty().WithMessage("Section Name is required.")
                .MaximumLength(50).WithMessage("Section Name cannot exceed 50 characters.")
                .Matches(@"^[A-Za-z0-9 -]+$").WithMessage("Section name may contain only letters, numbers, spaces, and hyphens.");

            RuleFor(x => x.RoomNumber)
                .MaximumLength(50).WithMessage("Room Number cannot exceed 50 characters.");

            RuleFor(x => x.InchargeId)
                .GreaterThan(0).When(x => x.InchargeId.HasValue).WithMessage("Valid Incharge ID is required.");

            RuleFor(x => x.MaximumStrength)
                .GreaterThan(0).WithMessage("Capacity is required and must be greater than 0.")
                .InclusiveBetween(1, 150).WithMessage("Capacity must be between 1 and 150.");
        }
    }
}
