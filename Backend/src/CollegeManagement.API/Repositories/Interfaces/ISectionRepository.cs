using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Sections;
using CollegeManagement.API.Models;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface ISectionRepository
    {
        Task<IEnumerable<SectionResponse>> GetAllSectionsAsync();
        Task<SectionResponse?> GetSectionByIdAsync(int id);
        Task<int> CreateSectionAsync(Section section);
        Task<bool> UpdateSectionAsync(int id, Section section);
        Task<bool> DeleteSectionAsync(int id);
        Task<IEnumerable<SectionResponse>> GetSectionsByGroupAsync(int groupId);
        Task<bool> IsSectionNameDuplicateAsync(string board, int academicYearId, string group, string academicLevel, string sectionName, int? excludeSectionId = null);
        Task<bool> AcademicYearExistsAsync(int academicYearId);
        Task<AcademicYear?> GetAcademicYearByIdAsync(int academicYearId);
        Task<bool> FacultyExistsAsync(int facultyId);
        Task<bool> RoomExistsAsync(int roomId);
    }
}
