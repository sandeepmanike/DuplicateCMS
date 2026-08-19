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
                .Must(x => x.SubjectId > 0 || !string.IsNullOrWhiteSpace(x.Subject) || !string.IsNullOrWhiteSpace(x.SubjectName) || !string.IsNullOrWhiteSpace(x.SubjectCode))
                .WithMessage("Valid Subject ID or Subject Name is required.");
        }
    }

    public class UpdateSubjectAllocationDtoValidator : AbstractValidator<UpdateSubjectAllocationDto>
    {
        public UpdateSubjectAllocationDtoValidator()
        {
            RuleFor(x => x)
                .Must(x => x.SubjectId > 0 || !string.IsNullOrWhiteSpace(x.Subject) || !string.IsNullOrWhiteSpace(x.SubjectName) || !string.IsNullOrWhiteSpace(x.SubjectCode))
                .WithMessage("Valid Subject ID or Subject Name is required.");
        }
    }
}
