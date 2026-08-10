using CollegeManagement.API.DTOs.Timetable;
using FluentValidation;

namespace CollegeManagement.API.Validators.TimetableValidators
{
    public class CreateRoomDtoValidator : AbstractValidator<CreateRoomDto>
    {
        public CreateRoomDtoValidator()
        {
            RuleFor(x => x.RoomCode)
                .NotEmpty().WithMessage("Room code is required.")
                .MaximumLength(30).WithMessage("Room code must not exceed 30 characters.");

            RuleFor(x => x.RoomName)
                .NotEmpty().WithMessage("Room name is required.")
                .MaximumLength(100).WithMessage("Room name must not exceed 100 characters.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0.");

            RuleFor(x => x.RoomType)
                .NotEmpty().WithMessage("Room type is required.")
                .MaximumLength(50).WithMessage("Room type must not exceed 50 characters.");
        }
    }

    public class UpdateRoomDtoValidator : AbstractValidator<UpdateRoomDto>
    {
        public UpdateRoomDtoValidator()
        {
            RuleFor(x => x.RoomCode)
                .NotEmpty().WithMessage("Room code is required.")
                .MaximumLength(30).WithMessage("Room code must not exceed 30 characters.");

            RuleFor(x => x.RoomName)
                .NotEmpty().WithMessage("Room name is required.")
                .MaximumLength(100).WithMessage("Room name must not exceed 100 characters.");

            RuleFor(x => x.Capacity)
                .GreaterThan(0).WithMessage("Capacity must be greater than 0.");

            RuleFor(x => x.RoomType)
                .NotEmpty().WithMessage("Room type is required.")
                .MaximumLength(50).WithMessage("Room type must not exceed 50 characters.");
        }
    }
}
