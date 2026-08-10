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
            var ids = await ResolveIdsAsync(board, academicYear, group, academicLevel, section, subject);

            var count = await Connection.ExecuteScalarAsync<int>(
                "sp_CheckDuplicateSubjectAllocation",
                new
                {
                    p_FacultyId = facultyId,
                    p_BoardId = ids.boardId,
                    p_AcademicYearId = ids.academicYearId,
                    p_GroupId = ids.groupId,
                    p_AcademicLevelId = ids.academicLevelId,
                    p_SectionId = ids.sectionId,
                    p_SubjectId = ids.subjectId,
                    p_ExcludeId = excludeId
                },
                commandType: CommandType.StoredProcedure);

            return count > 0;
        }

        public async Task<FacultySubjectAllocation> AddAsync(FacultySubjectAllocation allocation)
        {
            var ids = await ResolveIdsAsync(allocation.Board, allocation.AcademicYear, allocation.Group, allocation.AcademicLevel, allocation.Section, allocation.Subject);

            var id = await Connection.ExecuteScalarAsync<int>(
                "sp_CreateSubjectAllocation",
                new
                {
                    p_FacultyId = allocation.FacultyId,
                    p_BoardId = ids.boardId,
                    p_AcademicLevelId = ids.academicLevelId,
                    p_AcademicYearId = ids.academicYearId,
                    p_GroupId = ids.groupId,
                    p_SectionId = ids.sectionId,
                    p_SubjectId = ids.subjectId
                },
                commandType: CommandType.StoredProcedure);

            allocation.Id = id;
            return allocation;
        }

        public async Task UpdateAsync(FacultySubjectAllocation allocation)
        {
            var ids = await ResolveIdsAsync(allocation.Board, allocation.AcademicYear, allocation.Group, allocation.AcademicLevel, allocation.Section, allocation.Subject);

            await Connection.ExecuteAsync(
                "sp_UpdateSubjectAllocation",
                new
                {
                    p_Id = allocation.Id,
                    p_BoardId = ids.boardId,
                    p_AcademicYearId = ids.academicYearId,
                    p_GroupId = ids.groupId,
                    p_AcademicLevelId = ids.academicLevelId,
                    p_SectionId = ids.sectionId,
                    p_SubjectId = ids.subjectId
                },
                commandType: CommandType.StoredProcedure);
        }

        private async Task<(int boardId, int academicYearId, int groupId, int academicLevelId, int sectionId, int subjectId)> ResolveIdsAsync(
            string board, string academicYear, string group, string academicLevel, string section, string subject)
        {
            if (!int.TryParse(board, out int boardId) || boardId == 0)
            {
                boardId = await Connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT BoardId FROM Boards WHERE BoardCode = @val OR BoardName = @val LIMIT 1",
                    new { val = board });
            }

            if (!int.TryParse(academicYear, out int academicYearId) || academicYearId == 0)
            {
                academicYearId = await Connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT AcademicYearId FROM AcademicYears WHERE AcademicYearName = @val LIMIT 1",
                    new { val = academicYear });
            }

            if (!int.TryParse(group, out int groupId) || groupId == 0)
            {
                groupId = await Connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT GroupId FROM `Groups` WHERE GroupCode = @val OR GroupName = @val LIMIT 1",
                    new { val = group });
            }

            if (!int.TryParse(academicLevel, out int academicLevelId) || academicLevelId == 0)
            {
                academicLevelId = await Connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT AcademicLevelId FROM AcademicLevels WHERE LevelCode = @val OR LevelName = @val LIMIT 1",
                    new { val = academicLevel });
            }

            if (!int.TryParse(section, out int sectionId) || sectionId == 0)
            {
                sectionId = await Connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT SectionId FROM Sections WHERE SectionName = @val AND GroupId = @gId LIMIT 1",
                    new { val = section, gId = groupId });
                if (sectionId == 0)
                {
                    sectionId = await Connection.QueryFirstOrDefaultAsync<int>(
                        "SELECT SectionId FROM Sections WHERE SectionName = @val LIMIT 1",
                        new { val = section });
                }
            }

            if (!int.TryParse(subject, out int subjectId) || subjectId == 0)
            {
                subjectId = await Connection.QueryFirstOrDefaultAsync<int>(
                    "SELECT SubjectId FROM Subjects WHERE SubjectCode = @val OR SubjectName = @val LIMIT 1",
                    new { val = subject });
            }

            return (boardId, academicYearId, groupId, academicLevelId, sectionId, subjectId);
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
