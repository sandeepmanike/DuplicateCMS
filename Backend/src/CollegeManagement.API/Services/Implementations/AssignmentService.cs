using CollegeManagement.API.DTOs.Assignment;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services.Implementations
{
    public class AssignmentService : IAssignmentService
    {
        private readonly IAssignmentRepository _repository;

        public AssignmentService(IAssignmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AssignmentResponseDto>> GetAllAsync()
        {
            var assignments = await _repository.GetAllAsync();

            return assignments.Select(MapToResponseDto);
        }

        public async Task<AssignmentResponseDto?> GetByIdAsync(int id)
        {
            var assignment = await _repository.GetByIdAsync(id);

            if (assignment == null)
                return null;

            return MapToResponseDto(assignment);
        }

        public async Task<AssignmentResponseDto> CreateAsync(CreateAssignmentDto dto)
        {
            var assignment = new Assignment
            {
                Title = dto.Title,
                AcademicYearId = dto.AcademicYearId,
                AcademicLevel = dto.AcademicLevel,
                SubjectId = dto.SubjectId,
                FacultyId = dto.FacultyId,
                Description = dto.Description,
                DueDate = dto.DueDate,
                Attachment = dto.AttachmentPath,
                MaximumMarks = dto.MaximumMarks
            };

            await _repository.AddAsync(assignment);

            return MapToResponseDto(assignment);
        }

        public async Task<AssignmentResponseDto?> UpdateAsync(int id, UpdateAssignmentDto dto)
        {
            var assignment = await _repository.GetByIdAsync(id);

            if (assignment == null)
                return null;

            assignment.Title = dto.Title;
            assignment.AcademicYearId = dto.AcademicYearId;
            assignment.AcademicLevel = dto.AcademicLevel;
            assignment.SubjectId = dto.SubjectId;
            assignment.FacultyId = dto.FacultyId;
            assignment.Description = dto.Description;
            assignment.DueDate = dto.DueDate;
            assignment.MaximumMarks = dto.MaximumMarks;

            if (!string.IsNullOrEmpty(dto.AttachmentPath))
            {
                assignment.Attachment = dto.AttachmentPath;
            }

            await _repository.UpdateAsync(assignment);

            return MapToResponseDto(assignment);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _repository.GetByIdAsync(id);

            if (assignment == null)
                return false;

            await _repository.DeleteAsync(assignment);

            return true;
        }

        public async Task<bool> SubmitAssignmentAsync(int assignmentId, SubmitAssignmentDto dto)
        {
            var assignment = await _repository.GetByIdAsync(assignmentId);

            if (assignment == null)
                return false;

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(),
                                                "uploads",
                                                "submissions");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string fileName = Guid.NewGuid() +
                              Path.GetExtension(dto.SubmissionFile.FileName);

            string filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.SubmissionFile.CopyToAsync(stream);
            }

            var submission = new AssignmentSubmission
            {
                AssignmentId = assignmentId,
                StudentName = dto.StudentName,
                SubmissionFile = fileName
            };

            await _repository.SubmitAssignmentAsync(submission);

            return true;
        }

        public async Task<IEnumerable<AssignmentSubmissionResponseDto>> GetSubmissionsAsync(int assignmentId)
        {
            var submissions = await _repository.GetSubmissionsAsync(assignmentId);

            return submissions.Select(x => new AssignmentSubmissionResponseDto
            {
                AssignmentSubmissionId = x.AssignmentSubmissionId,
                AssignmentId = x.AssignmentId,
                StudentName = x.StudentName,
                SubmissionFile = x.SubmissionFile,
                SubmittedAt = x.SubmittedAt
            });
        }

        private AssignmentResponseDto MapToResponseDto(Assignment assignment)
        {
            return new AssignmentResponseDto
            {
                AssignmentId = assignment.AssignmentId,
                Title = assignment.Title,
                AcademicYearId = assignment.AcademicYearId,
                AcademicLevel = assignment.AcademicLevel,
                SubjectId = assignment.SubjectId,
                FacultyId = assignment.FacultyId,
                Description = assignment.Description,
                DueDate = assignment.DueDate,
                Attachment = assignment.Attachment,
                MaximumMarks = assignment.MaximumMarks
            };
        }
    }
}