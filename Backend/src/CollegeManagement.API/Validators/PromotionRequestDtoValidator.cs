using CollegeManagement.API.DTOs.Promotion;
using FluentValidation;

namespace CollegeManagement.API.Validators.PromotionModuleValidators
{
    public class PromotionRequestDtoValidator
        : AbstractValidator<PromotionRequestDto>
    {
        public PromotionRequestDtoValidator()
        {
            RuleFor(x => x.StudentIds)
                .NotEmpty()
                .WithMessage("Select at least one student.");

            RuleFor(x => x.NewClassId)
                .GreaterThan(0);

            RuleFor(x => x.AcademicYearId)
                .GreaterThan(0);
        }
    }
}