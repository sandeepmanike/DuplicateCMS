using CollegeManagement.API.DTOs.Faculty;
using FluentValidation;

namespace CollegeManagement.API.Validators.FacultyModuleValidators
{
    public class CreateDesignationDtoValidator : AbstractValidator<CreateDesignationDto>
    {
        public CreateDesignationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Designation name is required.")
                .MaximumLength(100).WithMessage("Designation name cannot exceed 100 characters.");
        }
    }

    public class UpdateDesignationDtoValidator : AbstractValidator<UpdateDesignationDto>
    {
        public UpdateDesignationDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Designation name is required.")
                .MaximumLength(100).WithMessage("Designation name cannot exceed 100 characters.");
        }
    }
}
