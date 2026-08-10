using System.Data;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models.Faculty;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class FacultySubjectAllocationRepository : IFacultySubjectAllocationRepository
    {
        private readonly AppDbContext _context;

        public FacultySubjectAllocationRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<FacultySubjectAllocation?> GetByIdAsync(int id)
        {
            var result = await Connection.QueryAsync<FacultySubjectAllocation, Faculty, FacultySubjectAllocation>(
                "sp_GetSubjectAllocationById",
                (allocation, faculty) =>
                {
                    allocation.Faculty = faculty;
                    return allocation;
                },
                new { p_Id = id },
                commandType: CommandType.StoredProcedure,
                splitOn: "Id");

            return result.FirstOrDefault();
        }

        public async Task<List<FacultySubjectAllocation>> GetByFacultyIdAsync(int facultyId)
        {
            var result = await Connection.QueryAsync<FacultySubjectAllocation, Faculty, FacultySubjectAllocation>(
                "sp_GetSubjectAllocationsByFacultyId",
                (allocation, faculty) =>
                {
                    allocation.Faculty = faculty;
                    return allocation;
                },
                new { p_FacultyId = facultyId },
                commandType: CommandType.StoredProcedure,
                splitOn: "Id");

            return result.ToList();
        }

        public async Task<bool> ExistsAllocationAsync(int facultyId, string board, string academicYear, string group, string academicLevel, string section, string subject, int? excludeId = null)
        {
            int.TryParse(board, out int boardId);
            int.TryParse(academicYear, out int academicYearId);
            int.TryParse(group, out int groupId);
            int.TryParse(academicLevel, out int academicLevelId);
            int.TryParse(section, out int sectionId);
            int.TryParse(subject, out int subjectId);

            var count = await Connection.ExecuteScalarAsync<int>(
                "sp_CheckDuplicateSubjectAllocation",
                new
                {
                    p_FacultyId = facultyId,
                    p_BoardId = boardId,
                    p_AcademicYearId = academicYearId,
                    p_GroupId = groupId,
                    p_AcademicLevelId = academicLevelId,
                    p_SectionId = sectionId,
                    p_SubjectId = subjectId,
                    p_ExcludeId = excludeId
                },
                commandType: CommandType.StoredProcedure);

            return count > 0;
        }

        public async Task<FacultySubjectAllocation> AddAsync(FacultySubjectAllocation allocation)
        {
            int.TryParse(allocation.Board, out int boardId);
            int.TryParse(allocation.AcademicYear, out int academicYearId);
            int.TryParse(allocation.Group, out int groupId);
            int.TryParse(allocation.AcademicLevel, out int academicLevelId);
            int.TryParse(allocation.Section, out int sectionId);
            int.TryParse(allocation.Subject, out int subjectId);

            var id = await Connection.ExecuteScalarAsync<int>(
                "sp_CreateSubjectAllocation",
                new
                {
                    p_FacultyId = allocation.FacultyId,
                    p_BoardId = boardId,
                    p_AcademicYearId = academicYearId,
                    p_GroupId = groupId,
                    p_AcademicLevelId = academicLevelId,
                    p_SectionId = sectionId,
                    p_SubjectId = subjectId
                },
                commandType: CommandType.StoredProcedure);

            allocation.Id = id;
            return allocation;
        }

        public async Task UpdateAsync(FacultySubjectAllocation allocation)
        {
            int.TryParse(allocation.Board, out int boardId);
            int.TryParse(allocation.AcademicYear, out int academicYearId);
            int.TryParse(allocation.Group, out int groupId);
            int.TryParse(allocation.AcademicLevel, out int academicLevelId);
            int.TryParse(allocation.Section, out int sectionId);
            int.TryParse(allocation.Subject, out int subjectId);

            await Connection.ExecuteAsync(
                "sp_UpdateSubjectAllocation",
                new
                {
                    p_Id = allocation.Id,
                    p_BoardId = boardId,
                    p_AcademicYearId = academicYearId,
                    p_GroupId = groupId,
                    p_AcademicLevelId = academicLevelId,
                    p_SectionId = sectionId,
                    p_SubjectId = subjectId
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task DeleteAsync(FacultySubjectAllocation allocation)
        {
            await Connection.ExecuteAsync(
                "sp_DeleteSubjectAllocation",
                new { p_Id = allocation.Id },
                commandType: CommandType.StoredProcedure);
        }
    }
}
