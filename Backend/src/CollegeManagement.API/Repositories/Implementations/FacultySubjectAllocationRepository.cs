using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<int?> ResolveSubjectIdAsync(int subjectId, string board, string academicYear, string group, string academicLevel, string section, string subject)
        {
            if (subjectId > 0)
            {
                return subjectId;
            }

            if (!string.IsNullOrWhiteSpace(subject))
            {
                var sName = subject.Trim();
                var foundId = await Connection.ExecuteScalarAsync<int?>(
                    "SELECT SubjectId FROM Subjects WHERE SubjectName = @Name OR SubjectCode = @Name LIMIT 1;",
                    new { Name = sName });

                if (foundId.HasValue && foundId.Value > 0)
                {
                    return foundId.Value;
                }
            }

            return null;
        }

        public async Task<FacultySubjectAllocation?> GetByIdAsync(int id)
        {
            try
            {
                var result = await Connection.QueryAsync<FacultySubjectAllocation, Faculty, Subject, FacultySubjectAllocation>(
                    "sp_GetSubjectAllocationById",
                    (allocation, faculty, subject) =>
                    {
                        allocation.Faculty = faculty;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { p_Id = id },
                    splitOn: "Id,SubjectId",
                    commandType: CommandType.StoredProcedure);

                return result.FirstOrDefault();
            }
            catch
            {
                const string sql = """
                    SELECT 
                        fsa.Id,
                        fsa.FacultyId,
                        fsa.SubjectId,
                        fsa.CreatedAt,
                        fsa.UpdatedAt,

                        f.Id,
                        f.EmployeeId,
                        f.FirstName,
                        f.LastName,
                        f.Email,

                        sub.SubjectId,
                        sub.SubjectCode,
                        sub.SubjectName,
                        sub.Board,
                        sub.Group,
                        sub.AcademicLevel
                    FROM FacultySubjectAllocations fsa
                    LEFT JOIN Faculties f ON f.Id = fsa.FacultyId
                    LEFT JOIN Subjects sub ON sub.SubjectId = fsa.SubjectId
                    WHERE fsa.Id = @Id;
                    """;

                var result = await Connection.QueryAsync<FacultySubjectAllocation, Faculty, Subject, FacultySubjectAllocation>(
                    sql,
                    (allocation, faculty, subject) =>
                    {
                        allocation.Faculty = faculty;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { Id = id },
                    splitOn: "Id,SubjectId");

                return result.FirstOrDefault();
            }
        }

        public async Task<List<FacultySubjectAllocation>> GetByFacultyIdAsync(int facultyId)
        {
            try
            {
                var result = await Connection.QueryAsync<FacultySubjectAllocation, Faculty, Subject, FacultySubjectAllocation>(
                    "sp_GetSubjectAllocationsByFacultyId",
                    (allocation, faculty, subject) =>
                    {
                        allocation.Faculty = faculty;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { p_FacultyId = facultyId },
                    splitOn: "Id,SubjectId",
                    commandType: CommandType.StoredProcedure);

                return result.ToList();
            }
            catch
            {
                const string sql = """
                    SELECT 
                        fsa.Id,
                        fsa.FacultyId,
                        fsa.SubjectId,
                        fsa.CreatedAt,
                        fsa.UpdatedAt,

                        f.Id,
                        f.EmployeeId,
                        f.FirstName,
                        f.LastName,
                        f.Email,

                        sub.SubjectId,
                        sub.SubjectCode,
                        sub.SubjectName,
                        sub.Board,
                        sub.Group,
                        sub.AcademicLevel
                    FROM FacultySubjectAllocations fsa
                    LEFT JOIN Faculties f ON f.Id = fsa.FacultyId
                    LEFT JOIN Subjects sub ON sub.SubjectId = fsa.SubjectId
                    WHERE fsa.FacultyId = @FacultyId
                    ORDER BY fsa.Id DESC;
                    """;

                var result = await Connection.QueryAsync<FacultySubjectAllocation, Faculty, Subject, FacultySubjectAllocation>(
                    sql,
                    (allocation, faculty, subject) =>
                    {
                        allocation.Faculty = faculty;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { FacultyId = facultyId },
                    splitOn: "Id,SubjectId");

                return result.ToList();
            }
        }

        public async Task<bool> ExistsAllocationAsync(int facultyId, int subjectId, int? excludeId = null)
        {
            int count = 0;
            try
            {
                count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckDuplicateSubjectAllocation",
                    new
                    {
                        p_FacultyId = facultyId,
                        p_SubjectId = subjectId,
                        p_ExcludeId = excludeId
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                const string sql = """
                    SELECT COUNT(1) FROM FacultySubjectAllocations
                    WHERE FacultyId = @FacultyId
                      AND SubjectId = @SubjectId
                      AND (@ExcludeId IS NULL OR Id <> @ExcludeId);
                    """;
                count = await Connection.ExecuteScalarAsync<int>(sql, new { FacultyId = facultyId, SubjectId = subjectId, ExcludeId = excludeId });
            }

            return count > 0;
        }

        public async Task<bool> ExistsAllocationAsync(int facultyId, string board, string academicYear, string group, string academicLevel, string section, string subject, int? excludeId = null)
        {
            var resolvedSubjectId = await ResolveSubjectIdAsync(0, board, academicYear, group, academicLevel, section, subject);
            if (!resolvedSubjectId.HasValue || resolvedSubjectId.Value <= 0)
            {
                return false;
            }

            return await ExistsAllocationAsync(facultyId, resolvedSubjectId.Value, excludeId);
        }

        public async Task<FacultySubjectAllocation> AddAsync(FacultySubjectAllocation allocation)
        {
            int id = 0;
            try
            {
                id = await Connection.ExecuteScalarAsync<int>(
                    "sp_CreateSubjectAllocation",
                    new
                    {
                        p_FacultyId = allocation.FacultyId,
                        p_SubjectId = allocation.SubjectId
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                const string sql = """
                    INSERT INTO FacultySubjectAllocations (FacultyId, SubjectId, CreatedAt)
                    VALUES (@FacultyId, @SubjectId, UTC_TIMESTAMP());
                    SELECT LAST_INSERT_ID();
                    """;
                id = await Connection.ExecuteScalarAsync<int>(sql, new { FacultyId = allocation.FacultyId, SubjectId = allocation.SubjectId });
            }

            allocation.Id = id;
            return allocation;
        }

        public async Task UpdateAsync(FacultySubjectAllocation allocation)
        {
            try
            {
                await Connection.ExecuteAsync(
                    "sp_UpdateSubjectAllocation",
                    new
                    {
                        p_Id = allocation.Id,
                        p_SubjectId = allocation.SubjectId
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                const string sql = """
                    UPDATE FacultySubjectAllocations 
                    SET SubjectId = @SubjectId, UpdatedAt = UTC_TIMESTAMP()
                    WHERE Id = @Id;
                    """;
                await Connection.ExecuteAsync(sql, new { Id = allocation.Id, SubjectId = allocation.SubjectId });
            }
        }

        public async Task DeleteAsync(FacultySubjectAllocation allocation)
        {
            try
            {
                await Connection.ExecuteAsync(
                    "sp_DeleteSubjectAllocation",
                    new { p_Id = allocation.Id },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                await Connection.ExecuteAsync("DELETE FROM FacultySubjectAllocations WHERE Id = @Id;", new { Id = allocation.Id });
            }
        }
    }
}
