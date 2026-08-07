using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.Models.Faculty;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IFacultySubjectAllocationRepository
    {
        Task<FacultySubjectAllocation?> GetByIdAsync(int id);
        Task<List<FacultySubjectAllocation>> GetByFacultyIdAsync(int facultyId);
        Task<bool> ExistsAllocationAsync(int facultyId, int boardId, int academicLevelId, int academicYearId, int groupId, int sectionId, int subjectId, int? excludeId = null);
        Task<FacultySubjectAllocation> AddAsync(FacultySubjectAllocation allocation);
        Task UpdateAsync(FacultySubjectAllocation allocation);
        Task DeleteAsync(FacultySubjectAllocation allocation);
    }
}
