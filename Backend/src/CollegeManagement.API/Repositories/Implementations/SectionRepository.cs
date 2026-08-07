using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Sections;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class SectionRepository : ISectionRepository
    {
        private readonly AppDbContext _context;

        public SectionRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<IEnumerable<SectionResponse>> GetAllSectionsAsync()
        {
            var result = await Connection.QueryAsync<SectionResponse>(
                "sp_GetAllSections",
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<SectionResponse?> GetSectionByIdAsync(int id)
        {
            return await Connection.QueryFirstOrDefaultAsync<SectionResponse>(
                "sp_GetSectionById",
                new { p_SectionId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateSectionAsync(Section section)
        {
            return await Connection.ExecuteScalarAsync<int>(
                "sp_CreateSection",
                new
                {
                    p_Board = section.Board,
                    p_AcademicYearId = section.AcademicYearId,
                    p_Group = section.Group,
                    p_AcademicLevel = section.AcademicLevel,
                    p_SectionName = section.SectionName,
                    p_RoomNumber = section.RoomNumber,
                    p_ClassTeacherId = section.ClassTeacherId,
                    p_MaximumStrength = section.MaximumStrength,
                    p_IsActive = section.IsActive
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateSectionAsync(int id, Section section)
        {
            var affected = await Connection.ExecuteAsync(
                "sp_UpdateSection",
                new
                {
                    p_SectionId = id,
                    p_Board = section.Board,
                    p_AcademicYearId = section.AcademicYearId,
                    p_Group = section.Group,
                    p_AcademicLevel = section.AcademicLevel,
                    p_SectionName = section.SectionName,
                    p_RoomNumber = section.RoomNumber,
                    p_ClassTeacherId = section.ClassTeacherId,
                    p_MaximumStrength = section.MaximumStrength,
                    p_IsActive = section.IsActive
                },
                commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        public async Task<bool> DeleteSectionAsync(int id)
        {
            var affected = await Connection.ExecuteAsync(
                "sp_DeleteSection",
                new { p_SectionId = id },
                commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        public async Task<IEnumerable<SectionResponse>> GetSectionsByGroupAsync(int groupId)
        {
            var result = await Connection.QueryAsync<SectionResponse>(
                "sp_GetSectionsByGroup",
                new { p_GroupId = groupId },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<bool> IsSectionNameDuplicateAsync(string board, int academicYearId, string group, string academicLevel, string sectionName, int? excludeSectionId = null)
        {
            var count = await Connection.ExecuteScalarAsync<int>(
                "sp_ValidateSectionName",
                new
                {
                    p_Board = board,
                    p_AcademicYearId = academicYearId,
                    p_Group = group,
                    p_AcademicLevel = academicLevel,
                    p_SectionName = sectionName,
                    p_ExcludeSectionId = excludeSectionId
                },
                commandType: CommandType.StoredProcedure);
            return count > 0;
        }

        public async Task<bool> AcademicYearExistsAsync(int academicYearId)
        {
            var count = await Connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM AcademicYears WHERE AcademicYearId = @Id",
                new { Id = academicYearId });
            return count > 0;
        }

        public async Task<bool> FacultyExistsAsync(int facultyId)
        {
            // First we try querying Faculty table
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Faculty WHERE Id = @Id AND IsDeleted = 0",
                    new { Id = facultyId });
                return count > 0;
            }
            catch
            {
                // In case the DB pluralized it to Faculties
                var count = await Connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Faculties WHERE Id = @Id AND IsDeleted = 0",
                    new { Id = facultyId });
                return count > 0;
            }
        }
    }
}
