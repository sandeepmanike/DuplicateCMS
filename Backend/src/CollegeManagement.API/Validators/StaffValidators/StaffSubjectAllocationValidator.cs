using CollegeManagement.API.DTOs.Staff;
using FluentValidation;

namespace CollegeManagement.API.Validators.StaffValidators
{
    public class AssignStaffSubjectDtoValidator : AbstractValidator<AssignStaffSubjectDto>
    {
        public AssignStaffSubjectDtoValidator()
        {
            RuleFor(x => x.StaffId)
                .GreaterThan(0).WithMessage("Valid Staff ID is required.");
        }
    }
}
