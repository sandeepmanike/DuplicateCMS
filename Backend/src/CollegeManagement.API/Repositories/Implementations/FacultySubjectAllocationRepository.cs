using System.Data;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models;
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
            var types = new[]
            {
                typeof(FacultySubjectAllocation),
                typeof(Faculty),
                typeof(Board),
                typeof(AcademicLevel),
                typeof(AcademicYear),
                typeof(Group),
                typeof(Section),
                typeof(Subject)
            };

            var result = await Connection.QueryAsync<FacultySubjectAllocation>(
                "sp_GetSubjectAllocationById",
                types,
                objects =>
                {
                    var allocation = (FacultySubjectAllocation)objects[0];
                    allocation.Faculty = (Faculty)objects[1];
                    allocation.Board = (Board)objects[2];
                    allocation.AcademicLevel = (AcademicLevel)objects[3];
                    allocation.AcademicYear = (AcademicYear)objects[4];
                    allocation.Group = (Group)objects[5];
                    allocation.Section = (Section)objects[6];
                    allocation.Subject = (Subject)objects[7];
                    return allocation;
                },
                new { p_Id = id },
                commandType: CommandType.StoredProcedure,
                splitOn: "Id,BoardId,AcademicLevelId,AcademicYearId,GroupId,SectionId,SubjectId");

            return result.FirstOrDefault();
        }

        public async Task<List<FacultySubjectAllocation>> GetByFacultyIdAsync(int facultyId)
        {
            var types = new[]
            {
                typeof(FacultySubjectAllocation),
                typeof(Faculty),
                typeof(Board),
                typeof(AcademicLevel),
                typeof(AcademicYear),
                typeof(Group),
                typeof(Section),
                typeof(Subject)
            };

            var result = await Connection.QueryAsync<FacultySubjectAllocation>(
                "sp_GetSubjectAllocationsByFacultyId",
                types,
                objects =>
                {
                    var allocation = (FacultySubjectAllocation)objects[0];
                    allocation.Faculty = (Faculty)objects[1];
                    allocation.Board = (Board)objects[2];
                    allocation.AcademicLevel = (AcademicLevel)objects[3];
                    allocation.AcademicYear = (AcademicYear)objects[4];
                    allocation.Group = (Group)objects[5];
                    allocation.Section = (Section)objects[6];
                    allocation.Subject = (Subject)objects[7];
                    return allocation;
                },
                new { p_FacultyId = facultyId },
                commandType: CommandType.StoredProcedure,
                splitOn: "Id,BoardId,AcademicLevelId,AcademicYearId,GroupId,SectionId,SubjectId");

            return result.ToList();
        }

        public async Task<bool> ExistsAllocationAsync(int facultyId, int boardId, int academicLevelId, int academicYearId, int groupId, int sectionId, int subjectId, int? excludeId = null)
        {
            var count = await Connection.ExecuteScalarAsync<int>(
                "sp_CheckDuplicateSubjectAllocation",
                new
                {
                    p_FacultyId = facultyId,
                    p_BoardId = boardId,
                    p_AcademicLevelId = academicLevelId,
                    p_AcademicYearId = academicYearId,
                    p_GroupId = groupId,
                    p_SectionId = sectionId,
                    p_SubjectId = subjectId,
                    p_ExcludeId = excludeId
                },
                commandType: CommandType.StoredProcedure);

            return count > 0;
        }

        public async Task<FacultySubjectAllocation> AddAsync(FacultySubjectAllocation allocation)
        {
            var id = await Connection.ExecuteScalarAsync<int>(
                "sp_CreateSubjectAllocation",
                new
                {
                    p_FacultyId = allocation.FacultyId,
                    p_BoardId = allocation.BoardId,
                    p_AcademicLevelId = allocation.AcademicLevelId,
                    p_AcademicYearId = allocation.AcademicYearId,
                    p_GroupId = allocation.GroupId,
                    p_SectionId = allocation.SectionId,
                    p_SubjectId = allocation.SubjectId
                },
                commandType: CommandType.StoredProcedure);

            allocation.Id = id;
            return allocation;
        }

        public async Task UpdateAsync(FacultySubjectAllocation allocation)
        {
            await Connection.ExecuteAsync(
                "sp_UpdateSubjectAllocation",
                new
                {
                    p_Id = allocation.Id,
                    p_BoardId = allocation.BoardId,
                    p_AcademicLevelId = allocation.AcademicLevelId,
                    p_AcademicYearId = allocation.AcademicYearId,
                    p_GroupId = allocation.GroupId,
                    p_SectionId = allocation.SectionId,
                    p_SubjectId = allocation.SubjectId
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
