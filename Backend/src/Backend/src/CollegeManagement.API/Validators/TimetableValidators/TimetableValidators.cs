using CollegeManagement.API.DTOs.Timetable;
using FluentValidation;

namespace CollegeManagement.API.Validators.TimetableValidators
{
    public class CreateTimetableDtoValidator : AbstractValidator<CreateTimetableDto>
    {
        public CreateTimetableDtoValidator()
        {
            RuleFor(x => x.BoardId).GreaterThan(0).WithMessage("Board ID is required.");
            RuleFor(x => x.AcademicLevelId).GreaterThan(0).WithMessage("Academic Level ID is required.");
            RuleFor(x => x.AcademicYearId).GreaterThan(0).WithMessage("Academic Year ID is required.");
            RuleFor(x => x.GroupId).GreaterThan(0).WithMessage("Group ID is required.");
            RuleFor(x => x.SectionId).GreaterThan(0).WithMessage("Section ID is required.");
            RuleFor(x => x.DayOfWeek).InclusiveBetween(1, 7).WithMessage("Day of week must be between 1 (Monday) and 7 (Sunday).");
            RuleFor(x => x.PeriodId).GreaterThan(0).WithMessage("Period ID is required.");
            RuleFor(x => x.SubjectId).GreaterThan(0).WithMessage("Subject ID is required.");
            RuleFor(x => x.FacultyId).GreaterThan(0).WithMessage("Faculty ID is required.");
            RuleFor(x => x.RoomId).GreaterThan(0).WithMessage("Room ID is required.");
            RuleFor(x => x.Remarks).MaximumLength(250).WithMessage("Remarks cannot exceed 250 characters.");
        }
    }

    public class UpdateTimetableDtoValidator : AbstractValidator<UpdateTimetableDto>
    {
        public UpdateTimetableDtoValidator()
        {
            RuleFor(x => x.BoardId).GreaterThan(0).WithMessage("Board ID is required.");
            RuleFor(x => x.AcademicLevelId).GreaterThan(0).WithMessage("Academic Level ID is required.");
            RuleFor(x => x.AcademicYearId).GreaterThan(0).WithMessage("Academic Year ID is required.");
            RuleFor(x => x.GroupId).GreaterThan(0).WithMessage("Group ID is required.");
            RuleFor(x => x.SectionId).GreaterThan(0).WithMessage("Section ID is required.");
            RuleFor(x => x.DayOfWeek).InclusiveBetween(1, 7).WithMessage("Day of week must be between 1 (Monday) and 7 (Sunday).");
            RuleFor(x => x.PeriodId).GreaterThan(0).WithMessage("Period ID is required.");
            RuleFor(x => x.SubjectId).GreaterThan(0).WithMessage("Subject ID is required.");
            RuleFor(x => x.FacultyId).GreaterThan(0).WithMessage("Faculty ID is required.");
            RuleFor(x => x.RoomId).GreaterThan(0).WithMessage("Room ID is required.");
            RuleFor(x => x.Remarks).MaximumLength(250).WithMessage("Remarks cannot exceed 250 characters.");
        }
    }

    public class CopyTimetableDtoValidator : AbstractValidator<CopyTimetableDto>
    {
        public CopyTimetableDtoValidator()
        {
            RuleFor(x => x.SourceAcademicYearId).GreaterThan(0).WithMessage("Source Academic Year ID is required.");
            RuleFor(x => x.SourceSectionId).GreaterThan(0).WithMessage("Source Section ID is required.");
            RuleFor(x => x.TargetAcademicYearId).GreaterThan(0).WithMessage("Target Academic Year ID is required.");
            RuleFor(x => x.TargetSectionId).GreaterThan(0).WithMessage("Target Section ID is required.");
        }
    }
}
