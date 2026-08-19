using CollegeManagement.API.DTOs.Subject;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Services
{
    public interface ISubjectService
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(int subjectId);
        Task<Subject> CreateAsync(CreateSubjectDto dto);
        Task<Subject?> UpdateAsync(int subjectId, UpdateSubjectDto dto);
        Task<bool> DeleteAsync(int subjectId);
        Task<IEnumerable<Subject>> GetByGroupIdAsync(int groupId);
        Task<IEnumerable<Subject>> SearchAsync(string? search, int? boardId, int? academicYearId, int? groupId, bool? isActive);
        Task<IEnumerable<Subject>> GetActiveAsync();
        Task<IEnumerable<Subject>> GetByBoardIdAsync(int boardId);
        Task<IEnumerable<Subject>> GetByAcademicYearIdAsync(int academicYearId);
        Task<bool> SubjectCodeExistsAsync(string subjectCode, int? excludeSubjectId = null);
    }
}
