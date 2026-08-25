using CollegeManagement.API.DTOs.Students;
using FluentValidation;

namespace CollegeManagement.API.Validators.StudentValidators
{
    public class ChangeSectionRequestValidator : AbstractValidator<ChangeSectionRequest>
    {
        public ChangeSectionRequestValidator()
        {
            RuleFor(x => x.SectionId)
                .GreaterThan(0).WithMessage("Valid SectionId is required.");
        }
    }

    public class ChangeGroupRequestValidator : AbstractValidator<ChangeGroupRequest>
    {
        public ChangeGroupRequestValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0).WithMessage("Valid GroupId is required.");

            RuleFor(x => x.SectionId)
                .GreaterThan(0).WithMessage("Valid SectionId within the target group is required.");
        }
    }

    public class TransferStudentRequestValidator : AbstractValidator<TransferStudentRequest>
    {
        public TransferStudentRequestValidator()
        {
            RuleFor(x => x.BoardId)
                .GreaterThan(0).WithMessage("Valid BoardId is required.");

            RuleFor(x => x.AcademicYearId)
                .GreaterThan(0).WithMessage("Valid AcademicYearId is required.");

            RuleFor(x => x.AcademicLevelId)
                .GreaterThan(0).WithMessage("Valid AcademicLevelId is required.");

            RuleFor(x => x.GroupId)
                .GreaterThan(0).WithMessage("Valid GroupId is required.");

            RuleFor(x => x.SectionId)
                .GreaterThan(0).WithMessage("Valid SectionId is required.");
        }
    }

    public class SuspendStudentRequestValidator : AbstractValidator<SuspendStudentRequest>
    {
        public SuspendStudentRequestValidator()
        {
            RuleFor(x => x.Reason)
                .MaximumLength(500).WithMessage("Reason cannot exceed 500 characters.");
        }
    }

    public class UpdateStudentProfileRequestValidator : AbstractValidator<UpdateStudentProfileRequest>
    {
        public UpdateStudentProfileRequestValidator()
        {
            RuleFor(x => x.StudentName)
                .NotEmpty().WithMessage("StudentName is required.")
                .MaximumLength(150).WithMessage("StudentName cannot exceed 150 characters.");

            When(x => !string.IsNullOrEmpty(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Invalid Email address.")
                    .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");
            });

            When(x => !string.IsNullOrEmpty(x.MobileNumber), () =>
            {
                RuleFor(x => x.MobileNumber)
                    .Matches(@"^[6-9][0-9]{9}$").WithMessage("MobileNumber must be a valid 10-digit number.");
            });

            When(x => !string.IsNullOrEmpty(x.AadhaarNumber), () =>
            {
                RuleFor(x => x.AadhaarNumber)
                    .Matches(@"^[0-9]{12}$").WithMessage("AadhaarNumber must be 12 digits.");
            });
        }
    }

    public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
    {
        public UpdateStudentRequestValidator()
        {
            RuleFor(x => x.StudentName)
                .NotEmpty().WithMessage("StudentName is required.")
                .MaximumLength(150).WithMessage("StudentName cannot exceed 150 characters.");

            RuleFor(x => x.BoardId)
                .GreaterThan(0).WithMessage("Valid BoardId is required.");

            RuleFor(x => x.AcademicYearId)
                .GreaterThan(0).WithMessage("Valid AcademicYearId is required.");

            RuleFor(x => x.AcademicLevelId)
                .GreaterThan(0).WithMessage("Valid AcademicLevelId is required.");

            RuleFor(x => x.GroupId)
                .GreaterThan(0).WithMessage("Valid GroupId is required.");

            RuleFor(x => x.SectionId)
                .GreaterThan(0).WithMessage("Valid SectionId is required.");

            When(x => !string.IsNullOrEmpty(x.Email), () =>
            {
                RuleFor(x => x.Email)
                    .EmailAddress().WithMessage("Invalid Email address.");
            });
        }
    }
}
