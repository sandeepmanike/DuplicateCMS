using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Faculty;
using CollegeManagement.API.Models.Faculty;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class FacultyRepository : IFacultyRepository
    {
        private readonly AppDbContext _context;

        public FacultyRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<Faculty?> GetByIdAsync(int id)
        {
            try
            {
                using var multi = await Connection.QueryMultipleAsync(
                    "sp_GetFacultyById",
                    new { p_Id = id },
                    commandType: CommandType.StoredProcedure);

                var faculty = await multi.ReadFirstOrDefaultAsync<Faculty>();
                if (faculty != null)
                {
                    var allocations = (await multi.ReadAsync<FacultySubjectAllocation>()).ToList();
                    faculty.FacultySubjectAllocations = allocations;
                }
                return faculty;
            }
            catch
            {
                var faculty = await _context.Faculties
                    .Include(f => f.FacultySubjectAllocations)
                    .FirstOrDefaultAsync(f => f.Id == id && !f.IsDeleted);

                if (faculty != null && faculty.DepartmentId.HasValue)
                {
                    var dept = await _context.Departments.FirstOrDefaultAsync(d => d.DepartmentId == faculty.DepartmentId.Value);
                    if (dept != null)
                    {
                        faculty.Department = dept.DepartmentName;
                    }
                }
                return faculty;
            }
        }

        public async Task<Faculty?> GetByEmployeeIdAsync(string employeeId)
        {
            try
            {
                return await Connection.QueryFirstOrDefaultAsync<Faculty>(
                    "sp_GetFacultyByEmployeeId",
                    new { p_EmployeeId = employeeId },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return await _context.Faculties.FirstOrDefaultAsync(f => f.EmployeeId == employeeId && !f.IsDeleted);
            }
        }

        public async Task<Faculty?> GetByEmailAsync(string email)
        {
            try
            {
                return await Connection.QueryFirstOrDefaultAsync<Faculty>(
                    "sp_GetFacultyByEmail",
                    new { p_Email = email },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return await _context.Faculties.FirstOrDefaultAsync(f => f.Email == email && !f.IsDeleted);
            }
        }

        public async Task<Faculty?> GetByMobileAsync(string mobile)
        {
            try
            {
                return await Connection.QueryFirstOrDefaultAsync<Faculty>(
                    "sp_GetFacultyByMobile",
                    new { p_Mobile = mobile },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return await _context.Faculties.FirstOrDefaultAsync(f => f.Mobile == mobile && !f.IsDeleted);
            }
        }

        public async Task<Faculty?> GetByAadhaarAsync(string aadhaar)
        {
            try
            {
                return await Connection.QueryFirstOrDefaultAsync<Faculty>(
                    "sp_GetFacultyByAadhaar",
                    new { p_Aadhaar = aadhaar },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                return await _context.Faculties.FirstOrDefaultAsync(f => f.Aadhaar == aadhaar && !f.IsDeleted);
            }
        }

        public async Task<string?> GetPhotoPathAsync(int id)
        {
            try
            {
                return await Connection.QueryFirstOrDefaultAsync<string?>(
                    "sp_GetFacultyPhotoPath",
                    new { p_Id = id },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                var f = await _context.Faculties.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
                return f?.PhotoPath;
            }
        }

        public async Task<bool> IsEmployeeIdUniqueAsync(string employeeId, int? excludeId = null)
        {
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckEmployeeIdUnique",
                    new { p_EmployeeId = employeeId, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count == 0;
            }
            catch
            {
                return !await _context.Faculties.AnyAsync(f => f.EmployeeId == employeeId && !f.IsDeleted && (excludeId == null || f.Id != excludeId));
            }
        }

        public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
        {
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckEmailUnique",
                    new { p_Email = email, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count == 0;
            }
            catch
            {
                return !await _context.Faculties.AnyAsync(f => f.Email == email && !f.IsDeleted && (excludeId == null || f.Id != excludeId));
            }
        }

        public async Task<bool> IsMobileUniqueAsync(string mobile, int? excludeId = null)
        {
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckMobileUnique",
                    new { p_Mobile = mobile, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count == 0;
            }
            catch
            {
                return !await _context.Faculties.AnyAsync(f => f.Mobile == mobile && !f.IsDeleted && (excludeId == null || f.Id != excludeId));
            }
        }

        public async Task<bool> IsAadhaarUniqueAsync(string aadhaar, int? excludeId = null)
        {
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckAadhaarUnique",
                    new { p_Aadhaar = aadhaar, p_ExcludeId = excludeId },
                    commandType: CommandType.StoredProcedure);

                return count == 0;
            }
            catch
            {
                return !await _context.Faculties.AnyAsync(f => f.Aadhaar == aadhaar && !f.IsDeleted && (excludeId == null || f.Id != excludeId));
            }
        }

        public async Task<(List<Faculty> Items, int TotalCount)> GetPagedFacultiesAsync(FacultyQueryParams queryParams)
        {
            try
            {
                using var multi = await Connection.QueryMultipleAsync(
                    "sp_GetPagedFaculties",
                    new
                    {
                        p_SearchTerm = queryParams.SearchTerm,
                        p_Department = queryParams.Department,
                        p_Designation = queryParams.Designation,
                        p_Status = queryParams.Status,
                        p_SortBy = queryParams.SortBy,
                        p_SortOrder = queryParams.SortOrder,
                        p_PageNumber = queryParams.PageNumber,
                        p_PageSize = queryParams.PageSize
                    },
                    commandType: CommandType.StoredProcedure);

                var totalCount = await multi.ReadFirstOrDefaultAsync<int>();
                var items = (await multi.ReadAsync<Faculty>()).ToList();

                return (items, totalCount);
            }
            catch
            {
                var query = _context.Faculties.Where(f => !f.IsDeleted);

                if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm))
                {
                    var term = queryParams.SearchTerm.Trim();
                    query = query.Where(f =>
                        f.FirstName.Contains(term) ||
                        f.LastName.Contains(term) ||
                        f.EmployeeId.Contains(term) ||
                        f.Email.Contains(term) ||
                        f.Mobile.Contains(term));
                }

                if (!string.IsNullOrWhiteSpace(queryParams.Designation))
                {
                    query = query.Where(f => f.Designation == queryParams.Designation.Trim());
                }

                if (!string.IsNullOrWhiteSpace(queryParams.Status))
                {
                    query = query.Where(f => f.Status == queryParams.Status.Trim());
                }

                var totalCount = await query.CountAsync();

                var pageNumber = queryParams.PageNumber > 0 ? queryParams.PageNumber : 1;
                var pageSize = queryParams.PageSize > 0 ? queryParams.PageSize : 10;

                var items = await query
                    .OrderByDescending(f => f.Id)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var deptIds = items.Where(f => f.DepartmentId.HasValue).Select(f => f.DepartmentId!.Value).Distinct().ToList();
                if (deptIds.Count > 0)
                {
                    var depts = await _context.Departments
                        .Where(d => deptIds.Contains(d.DepartmentId))
                        .ToDictionaryAsync(d => d.DepartmentId, d => d.DepartmentName);

                    foreach (var f in items)
                    {
                        if (f.DepartmentId.HasValue && depts.TryGetValue(f.DepartmentId.Value, out var dName))
                        {
                            f.Department = dName;
                        }
                    }
                }

                return (items, totalCount);
            }
        }

        public async Task<Faculty> AddAsync(Faculty faculty)
        {
            try
            {
                var id = await Connection.ExecuteScalarAsync<int>(
                    "sp_CreateFaculty",
                    new
                    {
                        p_EmployeeId = faculty.EmployeeId,
                        p_FirstName = faculty.FirstName,
                        p_LastName = faculty.LastName,
                        p_Gender = faculty.Gender,
                        p_DateOfBirth = faculty.DateOfBirth,
                        p_Aadhaar = faculty.Aadhaar,
                        p_Mobile = faculty.Mobile,
                        p_Email = faculty.Email,
                        p_BloodGroup = faculty.BloodGroup,
                        p_Qualification = faculty.Qualification,
                        p_Designation = faculty.Designation,
                        p_FacultyType = faculty.FacultyType,
                        p_DepartmentId = faculty.DepartmentId,
                        p_JoiningDate = faculty.JoiningDate,
                        p_Experience = faculty.Experience,
                        p_Status = faculty.Status,
                        p_PhotoPath = faculty.PhotoPath
                    },
                    commandType: CommandType.StoredProcedure);

                faculty.Id = id;
                return faculty;
            }
            catch
            {
                await _context.Faculties.AddAsync(faculty);
                await _context.SaveChangesAsync();
                return faculty;
            }
        }

        public async Task UpdateAsync(Faculty faculty)
        {
            try
            {
                await Connection.ExecuteAsync(
                    "sp_UpdateFaculty",
                    new
                    {
                        p_Id = faculty.Id,
                        p_FirstName = faculty.FirstName,
                        p_LastName = faculty.LastName,
                        p_Gender = faculty.Gender,
                        p_DateOfBirth = faculty.DateOfBirth,
                        p_Aadhaar = faculty.Aadhaar,
                        p_Mobile = faculty.Mobile,
                        p_Email = faculty.Email,
                        p_BloodGroup = faculty.BloodGroup,
                        p_Qualification = faculty.Qualification,
                        p_Designation = faculty.Designation,
                        p_FacultyType = faculty.FacultyType,
                        p_DepartmentId = faculty.DepartmentId,
                        p_JoiningDate = faculty.JoiningDate,
                        p_Experience = faculty.Experience,
                        p_Status = faculty.Status,
                        p_PhotoPath = faculty.PhotoPath
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                _context.Faculties.Update(faculty);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdatePhotoPathAsync(int id, string photoPath)
        {
            try
            {
                await Connection.ExecuteAsync(
                    "sp_UpdateFacultyPhotoPath",
                    new
                    {
                        p_Id = id,
                        p_PhotoPath = photoPath
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                var f = await _context.Faculties.FindAsync(id);
                if (f != null)
                {
                    f.PhotoPath = photoPath;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task SoftDeleteAsync(Faculty faculty)
        {
            try
            {
                await Connection.ExecuteAsync(
                    "sp_SoftDeleteFaculty",
                    new { p_Id = faculty.Id },
                    commandType: CommandType.StoredProcedure);
            }
            catch
            {
                faculty.IsDeleted = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}
