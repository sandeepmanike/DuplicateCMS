using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Staff;
using CollegeManagement.API.Models.Staff;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;

        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }

        private bool IsRelational => _context.Database.ProviderName != null && !_context.Database.ProviderName.Contains("InMemory");

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<Staff?> GetByIdAsync(int id)
        {
            try
            {
                using var multi = await Connection.QueryMultipleAsync(
                    "sp_GetStaffById",
                    new { p_Id = id },
                    commandType: CommandType.StoredProcedure);

                var staff = await multi.ReadFirstOrDefaultAsync<Staff>();
                if (staff != null)
                {
                    var allocations = (await multi.ReadAsync<StaffSubjectAllocation>()).ToList();
                    staff.StaffSubjectAllocations = allocations;
                    return staff;
                }
            }
            catch
            {
            }

            var sql = @"
                SELECT 
                    f.Id, f.EmployeeId, f.FirstName, f.LastName, f.Gender, f.DateOfBirth,
                    f.Aadhaar, f.Mobile, f.Email, f.BloodGroup, f.Qualification, f.Designation,
                    f.DesignationId, IFNULL(f.StaffType, 'Teaching') AS StaffType, f.DepartmentId,
                    d.DepartmentName AS Department,
                    f.JoiningDate, f.Experience, f.Status, f.PhotoPath, f.CreatedAt, f.UpdatedAt, f.IsDeleted
                FROM Staffs f
                LEFT JOIN Departments d ON d.DepartmentId = f.DepartmentId
                WHERE f.Id = @id AND (f.IsDeleted = 0 OR f.IsDeleted IS NULL);";

            var item = await Connection.QueryFirstOrDefaultAsync<Staff>(sql, new { id });
            if (item != null)
            {
                var allocSql = @"
                    SELECT 
                        a.Id, a.StaffId, a.FacultyId, a.SubjectId, a.CreatedAt, a.UpdatedAt,
                        sub.SubjectId, sub.SubjectName, sub.SubjectCode, sub.SubjectType
                    FROM StaffSubjectAllocations a
                    INNER JOIN Subjects sub ON sub.SubjectId = a.SubjectId
                    WHERE a.StaffId = @id OR a.FacultyId = @id;";

                var allocs = (await Connection.QueryAsync<StaffSubjectAllocation, Models.Subject, StaffSubjectAllocation>(
                    allocSql,
                    (allocation, subject) =>
                    {
                        allocation.Subject = subject;
                        return allocation;
                    },
                    new { id },
                    splitOn: "SubjectId")).ToList();

                item.StaffSubjectAllocations = allocs;
            }

            return item;
        }

        public async Task<Staff?> GetByEmployeeIdAsync(string employeeId)
        {
            try
            {
                var s = await Connection.QueryFirstOrDefaultAsync<Staff>(
                    "sp_GetStaffByEmployeeId",
                    new { p_EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);

                if (s != null) return s;
            }
            catch
            {
            }

            var sql = @"
                SELECT 
                    f.Id, f.EmployeeId, f.FirstName, f.LastName, f.Gender, f.DateOfBirth,
                    f.Aadhaar, f.Mobile, f.Email, f.BloodGroup, f.Qualification, f.Designation,
                    f.DesignationId, IFNULL(f.StaffType, 'Teaching') AS StaffType, f.DepartmentId,
                    d.DepartmentName AS Department,
                    f.JoiningDate, f.Experience, f.Status, f.PhotoPath, f.CreatedAt, f.UpdatedAt, f.IsDeleted
                FROM Staffs f
                LEFT JOIN Departments d ON d.DepartmentId = f.DepartmentId
                WHERE f.EmployeeId = @employeeId AND (f.IsDeleted = 0 OR f.IsDeleted IS NULL);";

            return await Connection.QueryFirstOrDefaultAsync<Staff>(sql, new { employeeId });
        }

        public async Task<Staff?> GetByEmailAsync(string email)
        {
            try
            {
                var s = await Connection.QueryFirstOrDefaultAsync<Staff>(
                    "sp_GetStaffByEmail",
                    new { p_Email = email },
                    commandType: CommandType.StoredProcedure);

                if (s != null) return s;
            }
            catch
            {
            }

            var sql = @"
                SELECT 
                    f.Id, f.EmployeeId, f.FirstName, f.LastName, f.Gender, f.DateOfBirth,
                    f.Aadhaar, f.Mobile, f.Email, f.BloodGroup, f.Qualification, f.Designation,
                    f.DesignationId, IFNULL(f.StaffType, 'Teaching') AS StaffType, f.DepartmentId,
                    d.DepartmentName AS Department,
                    f.JoiningDate, f.Experience, f.Status, f.PhotoPath, f.CreatedAt, f.UpdatedAt, f.IsDeleted
                FROM Staffs f
                LEFT JOIN Departments d ON d.DepartmentId = f.DepartmentId
                WHERE f.Email = @email AND (f.IsDeleted = 0 OR f.IsDeleted IS NULL);";

            return await Connection.QueryFirstOrDefaultAsync<Staff>(sql, new { email });
        }

        public async Task<Staff?> GetByMobileAsync(string mobile)
        {
            try
            {
                var s = await Connection.QueryFirstOrDefaultAsync<Staff>(
                    "sp_GetStaffByMobile",
                    new { p_Mobile = mobile },
                    commandType: CommandType.StoredProcedure);

                if (s != null) return s;
            }
            catch
            {
            }

            var sql = @"
                SELECT 
                    f.Id, f.EmployeeId, f.FirstName, f.LastName, f.Gender, f.DateOfBirth,
                    f.Aadhaar, f.Mobile, f.Email, f.BloodGroup, f.Qualification, f.Designation,
                    f.DesignationId, IFNULL(f.StaffType, 'Teaching') AS StaffType, f.DepartmentId,
                    d.DepartmentName AS Department,
                    f.JoiningDate, f.Experience, f.Status, f.PhotoPath, f.CreatedAt, f.UpdatedAt, f.IsDeleted
                FROM Staffs f
                LEFT JOIN Departments d ON d.DepartmentId = f.DepartmentId
                WHERE f.Mobile = @mobile AND (f.IsDeleted = 0 OR f.IsDeleted IS NULL);";

            return await Connection.QueryFirstOrDefaultAsync<Staff>(sql, new { mobile });
        }

        public async Task<Staff?> GetByAadhaarAsync(string aadhaar)
        {
            try
            {
                var s = await Connection.QueryFirstOrDefaultAsync<Staff>(
                    "sp_GetStaffByAadhaar",
                    new { p_Aadhaar = aadhaar },
                    commandType: CommandType.StoredProcedure);

                if (s != null) return s;
            }
            catch
            {
            }

            var sql = @"
                SELECT 
                    f.Id, f.EmployeeId, f.FirstName, f.LastName, f.Gender, f.DateOfBirth,
                    f.Aadhaar, f.Mobile, f.Email, f.BloodGroup, f.Qualification, f.Designation,
                    f.DesignationId, IFNULL(f.StaffType, 'Teaching') AS StaffType, f.DepartmentId,
                    d.DepartmentName AS Department,
                    f.JoiningDate, f.Experience, f.Status, f.PhotoPath, f.CreatedAt, f.UpdatedAt, f.IsDeleted
                FROM Staffs f
                LEFT JOIN Departments d ON d.DepartmentId = f.DepartmentId
                WHERE f.Aadhaar = @aadhaar AND (f.IsDeleted = 0 OR f.IsDeleted IS NULL);";

            return await Connection.QueryFirstOrDefaultAsync<Staff>(sql, new { aadhaar });
        }

        public async Task<string?> GetPhotoPathAsync(int id)
        {
            try
            {
                return await Connection.QueryFirstOrDefaultAsync<string>(
                    "sp_GetStaffPhotoPath",
                    new { p_Id = id },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                var sql = "SELECT PhotoPath FROM Staffs WHERE Id = @id;";
                return await Connection.QueryFirstOrDefaultAsync<string>(sql, new { id });
            }
        }

        public async Task<bool> IsEmployeeIdUniqueAsync(string employeeId, int? excludeId = null)
        {
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckStaffEmployeeIdUnique",
                    new { p_EmployeeId = employeeId, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count == 0;
            }
            catch
            {
                var sql = "SELECT COUNT(*) FROM Staffs WHERE EmployeeId = @employeeId AND (IsDeleted = 0 OR IsDeleted IS NULL) AND (@excludeId IS NULL OR Id != @excludeId);";
                var count = await Connection.ExecuteScalarAsync<int>(sql, new { employeeId, excludeId });
                return count == 0;
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckStaffEmailUnique",
                    new { p_Email = email, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count == 0;
            }
            catch
            {
                var sql = "SELECT COUNT(*) FROM Staffs WHERE Email = @email AND (IsDeleted = 0 OR IsDeleted IS NULL) AND (@excludeId IS NULL OR Id != @excludeId);";
                var count = await Connection.ExecuteScalarAsync<int>(sql, new { email, excludeId });
                return count == 0;
            }
        }

        public async Task<bool> IsMobileUniqueAsync(string mobile, int? excludeId = null)
        {
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckStaffMobileUnique",
                    new { p_Mobile = mobile, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count == 0;
            }
            catch
            {
                var sql = "SELECT COUNT(*) FROM Staffs WHERE Mobile = @mobile AND (IsDeleted = 0 OR IsDeleted IS NULL) AND (@excludeId IS NULL OR Id != @excludeId);";
                var count = await Connection.ExecuteScalarAsync<int>(sql, new { mobile, excludeId });
                return count == 0;
            }
        }

        public async Task<bool> IsAadhaarUniqueAsync(string aadhaar, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(aadhaar)) return true;
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckStaffAadhaarUnique",
                    new { p_Aadhaar = aadhaar, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count == 0;
            }
            catch
            {
                var sql = "SELECT COUNT(*) FROM Staffs WHERE Aadhaar = @aadhaar AND (IsDeleted = 0 OR IsDeleted IS NULL) AND (@excludeId IS NULL OR Id != @excludeId);";
                var count = await Connection.ExecuteScalarAsync<int>(sql, new { aadhaar, excludeId });
                return count == 0;
            }
        }

        public async Task<(List<Staff> Items, int TotalCount)> GetPagedStaffAsync(StaffQueryParams queryParams)
        {
            try
            {
                using var multi = await Connection.QueryMultipleAsync(
                    "sp_GetPagedStaff",
                    new
                    {
                        p_SearchTerm = queryParams.SearchTerm,
                        p_Department = queryParams.Department,
                        p_Designation = queryParams.Designation,
                        p_DesignationId = queryParams.DesignationId,
                        p_StaffType = queryParams.StaffType,
                        p_Status = queryParams.Status,
                        p_SortBy = queryParams.SortBy,
                        p_SortOrder = queryParams.SortOrder,
                        p_PageNumber = queryParams.PageNumber,
                        p_PageSize = queryParams.PageSize
                    },
                    commandType: CommandType.StoredProcedure);

                var totalCount = await multi.ReadFirstOrDefaultAsync<int>();
                var items = (await multi.ReadAsync<Staff>()).ToList();

                if (items.Count > 0 || totalCount > 0)
                    return (items, totalCount);
            }
            catch
            {
            }

            var whereClauses = new List<string> { "(f.IsDeleted = 0 OR f.IsDeleted IS NULL)" };
            var param = new DynamicParameters();

            if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
            {
                var term = $"%{queryParams.SearchTerm.Trim()}%";
                whereClauses.Add("(f.FirstName LIKE @term OR f.LastName LIKE @term OR f.EmployeeId LIKE @term OR f.Email LIKE @term OR f.Mobile LIKE @term)");
                param.Add("term", term);
            }

            if (!string.IsNullOrWhiteSpace(queryParams.StaffType) && !string.Equals(queryParams.StaffType, "All", StringComparison.OrdinalIgnoreCase))
            {
                whereClauses.Add("(f.StaffType = @staffType OR f.FacultyType = @staffType)");
                param.Add("staffType", queryParams.StaffType.Trim());
            }

            if (queryParams.DesignationId.HasValue && queryParams.DesignationId.Value > 0)
            {
                whereClauses.Add("f.DesignationId = @desigId");
                param.Add("desigId", queryParams.DesignationId.Value);
            }
            else if (!string.IsNullOrWhiteSpace(queryParams.Designation))
            {
                whereClauses.Add("f.Designation = @desig");
                param.Add("desig", queryParams.Designation.Trim());
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Department))
            {
                whereClauses.Add("(d.DepartmentName = @dept OR d.DepartmentCode = @dept)");
                param.Add("dept", queryParams.Department.Trim());
            }

            if (!string.IsNullOrWhiteSpace(queryParams.Status) && !string.Equals(queryParams.Status, "All Status", StringComparison.OrdinalIgnoreCase))
            {
                whereClauses.Add("f.Status = @status");
                param.Add("status", queryParams.Status.Trim());
            }

            var whereSql = string.Join(" AND ", whereClauses);

            var countSql = $@"
                SELECT COUNT(*) 
                FROM Staffs f
                LEFT JOIN Departments d ON d.DepartmentId = f.DepartmentId
                WHERE {whereSql};";

            var total = await Connection.ExecuteScalarAsync<int>(countSql, param);

            var pageNumber = queryParams.PageNumber > 0 ? queryParams.PageNumber : 1;
            var pageSize = queryParams.PageSize > 0 ? queryParams.PageSize : 10;
            var offset = (pageNumber - 1) * pageSize;

            param.Add("offset", offset);
            param.Add("limit", pageSize);

            var itemsSql = $@"
                SELECT 
                    f.Id, f.EmployeeId, f.FirstName, f.LastName, f.Gender, f.DateOfBirth,
                    f.Aadhaar, f.Mobile, f.Email, f.BloodGroup, f.Qualification, f.Designation,
                    f.DesignationId, IFNULL(f.StaffType, 'Teaching') AS StaffType, f.DepartmentId,
                    d.DepartmentName AS Department,
                    f.JoiningDate, f.Experience, f.Status, f.PhotoPath, f.CreatedAt, f.UpdatedAt, f.IsDeleted
                FROM Staffs f
                LEFT JOIN Departments d ON d.DepartmentId = f.DepartmentId
                WHERE {whereSql}
                ORDER BY f.Id DESC
                LIMIT @limit OFFSET @offset;";

            var records = (await Connection.QueryAsync<Staff>(itemsSql, param)).ToList();
            return (records, total);
        }

        public async Task<IEnumerable<StaffDropdownDto>> GetStaffDropdownAsync(string? staffType = null)
        {
            try
            {
                return await Connection.QueryAsync<StaffDropdownDto>(
                    "sp_GetStaffDropdown",
                    new { p_StaffType = staffType },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                var sql = @"
                    SELECT 
                        Id,
                        EmployeeId,
                        CONCAT(FirstName, ' ', LastName) AS FullName,
                        Designation,
                        DesignationId,
                        IFNULL(StaffType, 'Teaching') AS StaffType
                    FROM Staffs
                    WHERE (IsDeleted = 0 OR IsDeleted IS NULL) AND Status = 'Active'
                      AND (@staffType IS NULL OR @staffType = '' OR @staffType = 'All' OR StaffType = @staffType OR FacultyType = @staffType)
                    ORDER BY FirstName ASC;";

                return await Connection.QueryAsync<StaffDropdownDto>(sql, new { staffType });
            }
        }

        public async Task<string> GenerateNextEmployeeIdAsync(string staffType)
        {
            var isNonTeaching = string.Equals(staffType?.Trim(), "Non-Teaching", StringComparison.OrdinalIgnoreCase);
            var prefix = isNonTeaching ? "PJCNTCH" : "PJCTCH";

            try
            {
                var nextId = await Connection.QueryFirstOrDefaultAsync<string>(
                    "sp_GenerateStaffEmployeeId",
                    new { p_StaffType = isNonTeaching ? "Non-Teaching" : "Teaching" },
                    commandType: CommandType.StoredProcedure);

                if (!string.IsNullOrWhiteSpace(nextId))
                    return nextId;
            }
            catch
            {
            }

            var sql = "SELECT EmployeeId FROM Staffs WHERE EmployeeId LIKE @pattern;";
            var existingIds = (await Connection.QueryAsync<string>(sql, new { pattern = $"{prefix}%" })).ToList();

            int maxSeq = 0;
            foreach (var id in existingIds)
            {
                if (id.Length > prefix.Length)
                {
                    var suffix = id.Substring(prefix.Length);
                    if (int.TryParse(suffix, out int num) && num > maxSeq)
                    {
                        maxSeq = num;
                    }
                }
            }

            return $"{prefix}{(maxSeq + 1).ToString("D4")}";
        }

        public async Task<Staff> AddAsync(Staff staff)
        {
            try
            {
                var id = await Connection.ExecuteScalarAsync<int>(
                    "sp_CreateStaff",
                    new
                    {
                        p_EmployeeId = staff.EmployeeId,
                        p_FirstName = staff.FirstName,
                        p_LastName = staff.LastName,
                        p_Gender = staff.Gender,
                        p_DateOfBirth = staff.DateOfBirth,
                        p_Aadhaar = staff.Aadhaar,
                        p_Mobile = staff.Mobile,
                        p_Email = staff.Email,
                        p_BloodGroup = staff.BloodGroup,
                        p_Qualification = staff.Qualification,
                        p_Designation = staff.Designation,
                        p_DesignationId = staff.DesignationId,
                        p_StaffType = staff.StaffType,
                        p_DepartmentId = staff.DepartmentId,
                        p_JoiningDate = staff.JoiningDate,
                        p_Experience = staff.Experience,
                        p_Status = staff.Status,
                        p_PhotoPath = staff.PhotoPath
                    },
                    commandType: CommandType.StoredProcedure);

                if (id > 0)
                {
                    staff.Id = id;
                    return staff;
                }
            }
            catch
            {
            }

            var sql = @"
                INSERT INTO Staffs (
                    EmployeeId, FirstName, LastName, Gender, DateOfBirth,
                    Aadhaar, Mobile, Email, BloodGroup, Qualification,
                    Designation, DesignationId, StaffType, FacultyType,
                    DepartmentId, JoiningDate, Experience, Status, PhotoPath,
                    CreatedAt, IsDeleted
                ) VALUES (
                    @EmployeeId, @FirstName, @LastName, @Gender, @DateOfBirth,
                    @Aadhaar, @Mobile, @Email, @BloodGroup, @Qualification,
                    @Designation, @DesignationId, @StaffType, @StaffType,
                    @DepartmentId, @JoiningDate, @Experience, @Status, @PhotoPath,
                    UTC_TIMESTAMP(), 0
                );
                SELECT LAST_INSERT_ID();";

            staff.Id = await Connection.ExecuteScalarAsync<int>(sql, staff);
            return staff;
        }

        public async Task UpdateAsync(Staff staff)
        {
            try
            {
                await Connection.ExecuteAsync(
                    "sp_UpdateStaff",
                    new
                    {
                        p_Id = staff.Id,
                        p_FirstName = staff.FirstName,
                        p_LastName = staff.LastName,
                        p_Gender = staff.Gender,
                        p_DateOfBirth = staff.DateOfBirth,
                        p_Aadhaar = staff.Aadhaar,
                        p_Mobile = staff.Mobile,
                        p_Email = staff.Email,
                        p_BloodGroup = staff.BloodGroup,
                        p_Qualification = staff.Qualification,
                        p_Designation = staff.Designation,
                        p_DesignationId = staff.DesignationId,
                        p_StaffType = staff.StaffType,
                        p_DepartmentId = staff.DepartmentId,
                        p_JoiningDate = staff.JoiningDate,
                        p_Experience = staff.Experience,
                        p_Status = staff.Status,
                        p_PhotoPath = staff.PhotoPath
                    },
                    commandType: CommandType.StoredProcedure);
                return;
            }
            catch
            {
            }

            var sql = @"
                UPDATE Staffs SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Gender = @Gender,
                    DateOfBirth = @DateOfBirth,
                    Aadhaar = @Aadhaar,
                    Mobile = @Mobile,
                    Email = @Email,
                    BloodGroup = @BloodGroup,
                    Qualification = @Qualification,
                    Designation = @Designation,
                    DesignationId = @DesignationId,
                    StaffType = @StaffType,
                    FacultyType = @StaffType,
                    DepartmentId = @DepartmentId,
                    JoiningDate = @JoiningDate,
                    Experience = @Experience,
                    Status = @Status,
                    PhotoPath = @PhotoPath,
                    UpdatedAt = UTC_TIMESTAMP()
                WHERE Id = @Id;";

            await Connection.ExecuteAsync(sql, staff);
        }

        public async Task UpdatePhotoPathAsync(int id, string photoPath)
        {
            try
            {
                await Connection.ExecuteAsync(
                    "sp_UpdateStaffPhotoPath",
                    new
                    {
                        p_Id = id,
                        p_PhotoPath = photoPath
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                var sql = "UPDATE Staffs SET PhotoPath = @photoPath, UpdatedAt = UTC_TIMESTAMP() WHERE Id = @id;";
                await Connection.ExecuteAsync(sql, new { id, photoPath });
            }
        }

        public async Task SoftDeleteAsync(Staff staff)
        {
            try
            {
                await Connection.ExecuteAsync(
                    "sp_SoftDeleteStaff",
                    new { p_Id = staff.Id },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                var sql = "UPDATE Staffs SET IsDeleted = 1, UpdatedAt = UTC_TIMESTAMP() WHERE Id = @Id;";
                await Connection.ExecuteAsync(sql, new { staff.Id });
            }
        }
    }
}
