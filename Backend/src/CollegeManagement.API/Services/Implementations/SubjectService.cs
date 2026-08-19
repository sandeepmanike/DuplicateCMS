using CollegeManagement.API.DTOs.Subject;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories;

namespace CollegeManagement.API.Services
{
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repository;
        public SubjectService(ISubjectRepository repository) => _repository = repository;
        public Task<IEnumerable<Subject>> GetAllAsync() => _repository.GetAllAsync();
        public Task<Subject?> GetByIdAsync(int subjectId) => _repository.GetByIdAsync(subjectId);
        public Task<IEnumerable<Subject>> GetByGroupIdAsync(int groupId) => _repository.GetByGroupIdAsync(groupId);
        public async Task<Subject> CreateAsync(CreateSubjectDto dto) => await _repository.CreateAsync(new Subject
        {
            BoardId = dto.BoardId,
            AcademicYearId = dto.AcademicYearId,
            AcademicLevelId = dto.AcademicLevelId,
            GroupId = dto.GroupId,
            SubjectName = dto.SubjectName,
            SubjectCode = dto.SubjectCode,
            SubjectType = dto.SubjectType,
            Theory = dto.Theory,
            Practical = dto.Practical,
            Language = dto.Language,
            Elective = dto.Elective,
            InternalMarks = dto.InternalMarks,
            PracticalMarks = dto.PracticalMarks,
            ExternalMarks = dto.ExternalMarks,
            TotalMarks = dto.TotalMarks,
            PassingMarks = dto.PassingMarks,
            IsActive = dto.IsActive
        });
        public async Task<Subject?> UpdateAsync(int subjectId, UpdateSubjectDto dto)
        {
            var existing = await _repository.GetByIdAsync(subjectId);
            if (existing == null) return null;
            existing.BoardId = dto.BoardId; existing.AcademicYearId = dto.AcademicYearId; existing.AcademicLevelId = dto.AcademicLevelId; existing.GroupId = dto.GroupId;
            existing.SubjectName = dto.SubjectName; existing.SubjectCode = dto.SubjectCode; existing.SubjectType = dto.SubjectType; existing.Theory = dto.Theory;
            existing.Practical = dto.Practical; existing.Language = dto.Language; existing.Elective = dto.Elective; existing.InternalMarks = dto.InternalMarks;
            existing.PracticalMarks = dto.PracticalMarks; existing.ExternalMarks = dto.ExternalMarks; existing.TotalMarks = dto.TotalMarks; existing.PassingMarks = dto.PassingMarks; existing.IsActive = dto.IsActive;
            return await _repository.UpdateAsync(subjectId, existing);
        }
        public Task<bool> DeleteAsync(int subjectId) => _repository.DeleteAsync(subjectId);
        public Task<IEnumerable<Subject>> SearchAsync(string? search, int? boardId, int? academicYearId, int? groupId, bool? isActive) => _repository.SearchAsync(search, boardId, academicYearId, groupId, isActive);
        public Task<IEnumerable<Subject>> GetActiveAsync() => _repository.GetActiveAsync();
        public Task<IEnumerable<Subject>> GetByBoardIdAsync(int boardId) => _repository.GetByBoardIdAsync(boardId);
        public Task<IEnumerable<Subject>> GetByAcademicYearIdAsync(int academicYearId) => _repository.GetByAcademicYearIdAsync(academicYearId);
        public Task<bool> SubjectCodeExistsAsync(string subjectCode, int? excludeSubjectId = null) => _repository.SubjectCodeExistsAsync(subjectCode, excludeSubjectId);
    }
}
