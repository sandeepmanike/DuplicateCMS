using System;
using CollegeManagement.API.DTOs.Attendance.Requests;
using FluentValidation;

namespace CollegeManagement.API.Validators.Attendance
{
    /// <summary>
    /// Validator for <see cref="UpdateAttendanceRequest"/> to ensure standard attendance update constraints.
    /// </summary>
    public class UpdateAttendanceRequestValidator : AbstractValidator<UpdateAttendanceRequest>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateAttendanceRequestValidator"/> class.
        /// </summary>
        public UpdateAttendanceRequestValidator()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.AttendanceId)
                .GreaterThan(0).WithMessage("Attendance ID must be greater than 0.");

            RuleFor(x => x.AttendanceDate)
                .NotEmpty().WithMessage("Attendance date is required.")
                .LessThanOrEqualTo(_ => DateTime.Today).WithMessage("Attendance date cannot be greater than today.");

            RuleFor(x => x.StudentId)
                .GreaterThan(0).WithMessage("Please select a student.");

            RuleFor(x => x.FacultyId)
                .GreaterThan(0).WithMessage("Please select a faculty.");

            RuleFor(x => x.BoardId)
                .GreaterThan(0).WithMessage("Please select a board.");

            RuleFor(x => x.AcademicYearId)
                .GreaterThan(0).WithMessage("Please select an academic year.");

            RuleFor(x => x.AcademicLevelId)
                .GreaterThan(0).WithMessage("Please select an academic level.");

            RuleFor(x => x.GroupId)
                .GreaterThan(0).WithMessage("Please select a group.");

            RuleFor(x => x.SectionId)
                .GreaterThan(0).WithMessage("Please select a section.");

            RuleFor(x => x.SubjectId)
                .GreaterThan(0).WithMessage("Please select a subject.");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Please select a valid attendance status.");

            RuleFor(x => x.Remarks)
                .MaximumLength(500).WithMessage("Remarks cannot exceed 500 characters.");
        }
    }
}
