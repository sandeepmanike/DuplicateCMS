using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Assignment;
using CollegeManagement.API.DTOs.Faculty;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class AssignmentRepository : IAssignmentRepository
    {
        private readonly AppDbContext _context;

        public AssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Assignment>> GetAllAsync()
        {
            return await _context.Assignments
                .FromSqlRaw("CALL sp_GetAllAssignments()")
                .ToListAsync();
        }

        public async Task<Assignment?> GetByIdAsync(int id)
        {
            var result = await _context.Assignments
                .FromSqlRaw("CALL sp_GetAssignmentById({0})", id)
                .ToListAsync();

            return result.FirstOrDefault();
        }

        public async Task AddAsync(Assignment assignment)
        {
            var result = await _context.Assignments
                .FromSqlRaw(
                    "CALL sp_CreateAssignment({0},{1},{2},{3},{4},{5},{6},{7},{8},{9})",
                    assignment.Title,
assignment.AcademicYearId,
assignment.AcademicLevel,
assignment.GroupId,
assignment.SubjectId,
assignment.FacultyId,
assignment.Description,
assignment.DueDate,
assignment.Attachment,
assignment.MaximumMarks)
                .ToListAsync();
        }

        public async Task UpdateAsync(Assignment assignment)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL sp_UpdateAssignment({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10})",
                assignment.AssignmentId,
assignment.Title,
assignment.AcademicYearId,
assignment.AcademicLevel,
assignment.GroupId,
assignment.SubjectId,
assignment.FacultyId,
assignment.Description,
assignment.DueDate,
assignment.Attachment,
assignment.MaximumMarks);
        }
        public async Task DeleteAsync(Assignment assignment)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL sp_DeleteAssignment({0})",
                assignment.AssignmentId);
        }

        // We will implement these after creating AssignmentSubmissions table

        public async Task SubmitAssignmentAsync(AssignmentSubmission submission)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "CALL sp_SubmitAssignment({0},{1},{2})",
                submission.AssignmentId,
                submission.StudentName,
                submission.SubmissionFile);
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsAsync(int assignmentId)
        {
            return await _context.AssignmentSubmissions
                .FromSqlRaw("CALL sp_GetAssignmentSubmissions({0})", assignmentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubjectDropdownDto>>
GetSubjectsByGroupAsync(int groupId)
        {
            var groupName = await _context.Groups
                .Where(x => x.GroupId == groupId)
                .Select(x => x.GroupName)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(groupName))
                return new List<SubjectDropdownDto>();

            return await _context.Subjects
                .Where(x => x.Group == groupName)
                .Select(x => new SubjectDropdownDto
                {
                    SubjectId = x.SubjectId,
                    SubjectName = x.SubjectName
                })
                .ToListAsync();
        }

        

        public async Task<IEnumerable<FacultyDropdownDto>>
GetFacultyBySubjectAsync(
    int subjectId,
    int groupId,
    int academicYearId,
    string academicLevel)
        {
            var groupName = await _context.Groups
                .Where(x => x.GroupId == groupId)
                .Select(x => x.GroupName)
                .FirstOrDefaultAsync();

            var academicYear = await _context.AcademicYears
                .Where(x => x.AcademicYearId == academicYearId)
                .Select(x => x.AcademicYearName)
                .FirstOrDefaultAsync();

            var subjectName = await _context.Subjects
                .Where(x => x.SubjectId == subjectId)
                .Select(x => x.SubjectName)
                .FirstOrDefaultAsync();

            return await
            (
                from allocation in _context.FacultySubjectAllocations
                join faculty in _context.Faculties
                    on allocation.FacultyId equals faculty.Id
                where allocation.Group == groupName
                   && allocation.Subject == subjectName
                   && allocation.AcademicYear == academicYear
                   && allocation.AcademicLevel == academicLevel
                select new FacultyDropdownDto
                {
                    Id = faculty.Id,
                    FullName = faculty.FirstName + " " + faculty.LastName
                }
            ).ToListAsync();
        }
    }
}