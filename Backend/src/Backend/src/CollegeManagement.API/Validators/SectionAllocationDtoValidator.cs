using CollegeManagement.API.DTOs.Promotion;
using FluentValidation;

namespace CollegeManagement.API.Validators.PromotionModuleValidators
{
    public class SectionAllocationDtoValidator
        : AbstractValidator<SectionAllocationDto>
    {
        public SectionAllocationDtoValidator()
        {
            RuleFor(x => x.SectionId)
                .GreaterThan(0);

            RuleFor(x => x.StudentIds)
                .NotEmpty();
        }
    }
}