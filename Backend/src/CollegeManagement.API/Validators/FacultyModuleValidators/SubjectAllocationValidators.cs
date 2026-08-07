using CollegeManagement.API.DTOs.Faculty.Request;
using FluentValidation;

namespace CollegeManagement.API.Validators.FacultyModuleValidators
{
    public class AssignSubjectDtoValidator : AbstractValidator<AssignSubjectDto>
    {
        public AssignSubjectDtoValidator()
        {
            RuleFor(x => x.FacultyId)
                .GreaterThan(0).WithMessage("Valid Faculty ID is required.");

            RuleFor(x => x)
                .Must(x => x.BoardId > 0 || !string.IsNullOrWhiteSpace(x.Board))
                .WithMessage("Board (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.AcademicLevelId > 0 || !string.IsNullOrWhiteSpace(x.AcademicLevel))
                .WithMessage("Academic Level (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.AcademicYearId > 0 || !string.IsNullOrWhiteSpace(x.AcademicYear))
                .WithMessage("Academic Year (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.GroupId > 0 || !string.IsNullOrWhiteSpace(x.Group))
                .WithMessage("Group (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.SectionId > 0 || !string.IsNullOrWhiteSpace(x.Section))
                .WithMessage("Section (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.SubjectId > 0 || !string.IsNullOrWhiteSpace(x.Subject))
                .WithMessage("Subject (ID or Name) is required.");
        }
    }

    public class UpdateSubjectAllocationDtoValidator : AbstractValidator<UpdateSubjectAllocationDto>
    {
        public UpdateSubjectAllocationDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => x.BoardId > 0 || !string.IsNullOrWhiteSpace(x.Board))
                .WithMessage("Board (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.AcademicLevelId > 0 || !string.IsNullOrWhiteSpace(x.AcademicLevel))
                .WithMessage("Academic Level (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.AcademicYearId > 0 || !string.IsNullOrWhiteSpace(x.AcademicYear))
                .WithMessage("Academic Year (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.GroupId > 0 || !string.IsNullOrWhiteSpace(x.Group))
                .WithMessage("Group (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.SectionId > 0 || !string.IsNullOrWhiteSpace(x.Section))
                .WithMessage("Section (ID or Name) is required.");

            RuleFor(x => x)
                .Must(x => x.SubjectId > 0 || !string.IsNullOrWhiteSpace(x.Subject))
                .WithMessage("Subject (ID or Name) is required.");
        }
    }
}
