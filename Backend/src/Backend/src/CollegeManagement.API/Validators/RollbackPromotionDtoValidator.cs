using CollegeManagement.API.DTOs.Promotion;
using FluentValidation;

namespace CollegeManagement.API.Validators.PromotionModuleValidators
{
    public class RollbackPromotionDtoValidator
        : AbstractValidator<RollbackPromotionDto>
    {
        public RollbackPromotionDtoValidator()
        {
            RuleFor(x => x.PromotionId)
                .GreaterThan(0);
        }
    }
}