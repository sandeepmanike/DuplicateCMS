using System;
using CollegeManagement.API.DTOs.Faculty.Request;
using FluentValidation;

namespace CollegeManagement.API.Validators.FacultyModuleValidators
{
    public class CreateFacultyDtoValidator : AbstractValidator<CreateFacultyDto>
    {
        public CreateFacultyDtoValidator()
        {
            RuleFor(x => x.EmployeeId)
                .NotEmpty().WithMessage("Employee ID is required.")
                .MaximumLength(50).WithMessage("Employee ID cannot exceed 50 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .Must(x => x == "Male" || x == "Female" || x == "Other")
                .WithMessage("Gender must be 'Male', 'Female', or 'Other'.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.")
                .Must(dob => dob <= DateTime.Today.AddYears(-18)).WithMessage("Faculty must be at least 18 years old.")
                .Must(dob => dob >= DateTime.Today.AddYears(-100)).WithMessage("Faculty age cannot exceed 100 years.");

            RuleFor(x => x.Aadhaar)
                .NotEmpty().WithMessage("Aadhaar number is required.")
                .Matches(@"^\d{12}$").WithMessage("Aadhaar number must be exactly 12 numeric digits.");

            RuleFor(x => x.Mobile)
                .NotEmpty().WithMessage("Mobile number is required.")
                .Matches(@"^\d{10,15}$").WithMessage("Mobile number must be between 10 and 15 digits.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email address is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email cannot exceed 150 characters.");

            RuleFor(x => x.Qualification)
                .NotEmpty().WithMessage("Qualification is required.")
                .MaximumLength(100).WithMessage("Qualification cannot exceed 100 characters.");

            RuleFor(x => x)
                .Must(x => (x.DesignationId.HasValue && x.DesignationId.Value > 0) || !string.IsNullOrWhiteSpace(x.Designation))
                .WithMessage("Designation is required.");

            RuleFor(x => x.Designation)
                .MaximumLength(100).WithMessage("Designation cannot exceed 100 characters.");

            RuleFor(x => x.FacultyType)
                .NotEmpty().WithMessage("Faculty type is required.")
                .Must(x => string.Equals(x, "Teaching", StringComparison.OrdinalIgnoreCase) || string.Equals(x, "Non-Teaching", StringComparison.OrdinalIgnoreCase))
                .WithMessage("FacultyType must be 'Teaching' or 'Non-Teaching'.");

            RuleFor(x => x)
                .Must(x => (x.DepartmentId.HasValue && x.DepartmentId.Value > 0) || !string.IsNullOrWhiteSpace(x.Department))
                .WithMessage("Department is required.");

            RuleFor(x => x.JoiningDate)
                .NotEmpty().WithMessage("Joining date is required.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Joining date cannot be in the future.");

            RuleFor(x => x.Experience)
                .GreaterThanOrEqualTo(0).WithMessage("Experience cannot be negative.");

            RuleFor(x => x.Status)
                .Must(x => string.Equals(x, "Active", StringComparison.OrdinalIgnoreCase) || string.Equals(x, "Inactive", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Status must be 'Active' or 'Inactive'.");
        }
    }
}
