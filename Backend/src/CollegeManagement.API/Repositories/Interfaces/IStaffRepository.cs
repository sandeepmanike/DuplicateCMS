using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Staff;
using CollegeManagement.API.Models.Staff;

namespace CollegeManagement.API.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        Task<Staff?> GetByIdAsync(int id);
        Task<Staff?> GetByEmployeeIdAsync(string employeeId);
        Task<Staff?> GetByEmailAsync(string email);
        Task<Staff?> GetByMobileAsync(string mobile);
        Task<Staff?> GetByAadhaarAsync(string aadhaar);
        Task<string?> GetPhotoPathAsync(int id);

        Task<bool> IsEmployeeIdUniqueAsync(string employeeId, int? excludeId = null);
        Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
        Task<bool> IsMobileUniqueAsync(string mobile, int? excludeId = null);
        Task<bool> IsAadhaarUniqueAsync(string aadhaar, int? excludeId = null);

        Task<(List<Staff> Items, int TotalCount)> GetPagedStaffAsync(StaffQueryParams queryParams);
        Task<IEnumerable<StaffDropdownDto>> GetStaffDropdownAsync(string? staffType = null);
        Task<string> GenerateNextEmployeeIdAsync(string staffType);

        Task<Staff> AddAsync(Staff staff);
        Task UpdateAsync(Staff staff);
        Task UpdatePhotoPathAsync(int id, string photoPath);
        Task SoftDeleteAsync(Staff staff);
    }
}
