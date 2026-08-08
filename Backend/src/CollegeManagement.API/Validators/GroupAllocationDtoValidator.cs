using CollegeManagement.API.DTOs.Promotion;
using FluentValidation;

namespace CollegeManagement.API.Validators.PromotionModuleValidators
{
    public class GroupAllocationDtoValidator
        : AbstractValidator<GroupAllocationDto>
    {
        public GroupAllocationDtoValidator()
        {
            RuleFor(x => x.GroupId)
                .GreaterThan(0);

            RuleFor(x => x.StudentIds)
                .NotEmpty();
        }
    }
}