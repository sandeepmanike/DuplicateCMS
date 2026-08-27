using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.Models;
using CollegeManagement.API.Models.Staff;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class StaffSubjectAllocationRepository : IStaffSubjectAllocationRepository
    {
        private readonly AppDbContext _context;

        public StaffSubjectAllocationRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<StaffSubjectAllocation?> GetByIdAsync(int id)
        {
            try
            {
                var list = await Connection.QueryAsync<StaffSubjectAllocation, Staff, Subject, StaffSubjectAllocation>(
                    "sp_GetSubjectAllocationById",
                    (allocation, staff, subject) =>
                    {
                        allocation.Staff = staff;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { p_Id = id },
                    splitOn: "StaffRecordId,SubjectId",
                    commandType: CommandType.StoredProcedure);

                return list.FirstOrDefault();
            }
            catch
            {
                var sql = @"
                    SELECT 
                        a.Id, a.StaffId, a.FacultyId, a.SubjectId, a.CreatedAt, a.UpdatedAt,
                        s.Id AS StaffRecordId, s.Id, s.EmployeeId, s.FirstName, s.LastName, s.Email, s.Mobile, s.Designation, s.StaffType,
                        sub.SubjectId, sub.SubjectName, sub.SubjectCode, sub.SubjectType
                    FROM StaffSubjectAllocations a
                    INNER JOIN Staffs s ON s.Id = COALESCE(a.StaffId, a.FacultyId)
                    INNER JOIN Subjects sub ON sub.SubjectId = a.SubjectId
                    WHERE a.Id = @id;";

                var list = await Connection.QueryAsync<StaffSubjectAllocation, Staff, Subject, StaffSubjectAllocation>(
                    sql,
                    (allocation, staff, subject) =>
                    {
                        allocation.Staff = staff;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { id },
                    splitOn: "StaffRecordId,SubjectId");

                return list.FirstOrDefault();
            }
        }

        public async Task<List<StaffSubjectAllocation>> GetByStaffIdAsync(int staffId)
        {
            try
            {
                var list = await Connection.QueryAsync<StaffSubjectAllocation, Staff, Subject, StaffSubjectAllocation>(
                    "sp_GetSubjectAllocationsByStaffId",
                    (allocation, staff, subject) =>
                    {
                        allocation.Staff = staff;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { p_StaffId = staffId },
                    splitOn: "StaffRecordId,SubjectId",
                    commandType: CommandType.StoredProcedure);

                return list.ToList();
            }
            catch
            {
                var sql = @"
                    SELECT 
                        a.Id, a.StaffId, a.FacultyId, a.SubjectId, a.CreatedAt, a.UpdatedAt,
                        s.Id AS StaffRecordId, s.Id, s.EmployeeId, s.FirstName, s.LastName, s.Email, s.Mobile, s.Designation, s.StaffType,
                        sub.SubjectId, sub.SubjectName, sub.SubjectCode, sub.SubjectType
                    FROM StaffSubjectAllocations a
                    INNER JOIN Staffs s ON s.Id = COALESCE(a.StaffId, a.FacultyId)
                    INNER JOIN Subjects sub ON sub.SubjectId = a.SubjectId
                    WHERE (a.StaffId = @staffId OR a.FacultyId = @staffId);";

                var list = await Connection.QueryAsync<StaffSubjectAllocation, Staff, Subject, StaffSubjectAllocation>(
                    sql,
                    (allocation, staff, subject) =>
                    {
                        allocation.Staff = staff;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { staffId },
                    splitOn: "StaffRecordId,SubjectId");

                return list.ToList();
            }
        }

        public async Task<List<StaffSubjectAllocation>> GetBySubjectIdAsync(int subjectId)
        {
            try
            {
                var list = await Connection.QueryAsync<StaffSubjectAllocation, Staff, Subject, StaffSubjectAllocation>(
                    "sp_GetSubjectAllocationsBySubjectId",
                    (allocation, staff, subject) =>
                    {
                        allocation.Staff = staff;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { p_SubjectId = subjectId },
                    splitOn: "StaffRecordId,SubjectId",
                    commandType: CommandType.StoredProcedure);

                return list.ToList();
            }
            catch
            {
                var sql = @"
                    SELECT 
                        a.Id, a.StaffId, a.FacultyId, a.SubjectId, a.CreatedAt, a.UpdatedAt,
                        s.Id AS StaffRecordId, s.Id, s.EmployeeId, s.FirstName, s.LastName, s.Email, s.Mobile, s.Designation, s.StaffType,
                        sub.SubjectId, sub.SubjectName, sub.SubjectCode, sub.SubjectType
                    FROM StaffSubjectAllocations a
                    INNER JOIN Staffs s ON s.Id = COALESCE(a.StaffId, a.FacultyId)
                    INNER JOIN Subjects sub ON sub.SubjectId = a.SubjectId
                    WHERE a.SubjectId = @subjectId;";

                var list = await Connection.QueryAsync<StaffSubjectAllocation, Staff, Subject, StaffSubjectAllocation>(
                    sql,
                    (allocation, staff, subject) =>
                    {
                        allocation.Staff = staff;
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { subjectId },
                    splitOn: "StaffRecordId,SubjectId");

                return list.ToList();
            }
        }

        public async Task<bool> ExistsAllocationAsync(int staffId, int subjectId, int? excludeId = null)
        {
            try
            {
                int count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckStaffSubjectAllocationExists",
                    new { p_StaffId = staffId, p_SubjectId = subjectId, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count > 0;
            }
            catch
            {
                var sql = @"
                    SELECT COUNT(*) FROM StaffSubjectAllocations 
                    WHERE (StaffId = @staffId OR FacultyId = @staffId) AND SubjectId = @subjectId 
                      AND (@excludeId IS NULL OR Id != @excludeId);";

                int count = await Connection.ExecuteScalarAsync<int>(sql, new { staffId, subjectId, excludeId });
                return count > 0;
            }
        }

        public async Task<int?> ResolveSubjectIdAsync(int? subjectId, string board, string academicYear, string group, string academicLevel, string section, string subjectName)
        {
            if (subjectId.HasValue && subjectId.Value > 0)
                return subjectId.Value;

            try
            {
                var id = await Connection.ExecuteScalarAsync<int?>(
                    "sp_ResolveSubjectId",
                    new
                    {
                        p_SubjectName = subjectName?.Trim(),
                        p_Board = board?.Trim(),
                        p_Group = group?.Trim(),
                        p_AcademicLevel = academicLevel?.Trim()
                    },
                    commandType: CommandType.StoredProcedure);

                if (id.HasValue && id.Value > 0) return id.Value;
            }
            catch
            {
            }

            var query = _context.Subjects.AsQueryable();
            if (!string.IsNullOrWhiteSpace(subjectName))
            {
                var name = subjectName.Trim().ToLower();
                var s = await query.FirstOrDefaultAsync(x => x.SubjectName.ToLower() == name || x.SubjectCode.ToLower() == name);
                if (s != null) return s.SubjectId;
            }

            var first = await query.FirstOrDefaultAsync();
            return first?.SubjectId;
        }

        public async Task<StaffSubjectAllocation> AddAsync(StaffSubjectAllocation allocation)
        {
            try
            {
                int id = await Connection.ExecuteScalarAsync<int>(
                    "sp_AssignStaffSubject",
                    new
                    {
                        p_StaffId = allocation.StaffId > 0 ? allocation.StaffId : (allocation.FacultyId ?? 0),
                        p_SubjectId = allocation.SubjectId
                    },
                    commandType: CommandType.StoredProcedure);

                allocation.Id = id;
                return allocation;
            }
            catch
            {
                int sid = allocation.StaffId > 0 ? allocation.StaffId : (allocation.FacultyId ?? 0);
                var insertSql = @"
                    INSERT INTO StaffSubjectAllocations (StaffId, FacultyId, SubjectId, CreatedAt)
                    VALUES (@sid, @sid, @subjectId, UTC_TIMESTAMP());
                    SELECT LAST_INSERT_ID();";

                int id = await Connection.ExecuteScalarAsync<int>(insertSql, new { sid, subjectId = allocation.SubjectId });
                allocation.Id = id;
                return allocation;
            }
        }

        public async Task UpdateAsync(StaffSubjectAllocation allocation)
        {
            int sid = allocation.StaffId > 0 ? allocation.StaffId : (allocation.FacultyId ?? 0);
            var sql = @"
                UPDATE StaffSubjectAllocations 
                SET StaffId = @sid, FacultyId = @sid, SubjectId = @subjectId, UpdatedAt = UTC_TIMESTAMP()
                WHERE Id = @id;";

            await Connection.ExecuteAsync(sql, new { id = allocation.Id, sid, subjectId = allocation.SubjectId });
        }

        public async Task DeleteAsync(StaffSubjectAllocation allocation)
        {
            var sql = "DELETE FROM StaffSubjectAllocations WHERE Id = @id;";
            await Connection.ExecuteAsync(sql, new { id = allocation.Id });
        }
    }
}
