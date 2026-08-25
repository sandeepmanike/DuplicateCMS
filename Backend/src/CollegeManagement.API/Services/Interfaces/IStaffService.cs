using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Staff;

namespace CollegeManagement.API.Services.Interfaces
{
    public interface IStaffService
    {
        Task<PagedResult<StaffResponseDto>> GetPagedStaffAsync(StaffQueryParams queryParams);
        Task<IEnumerable<StaffDropdownDto>> GetStaffDropdownAsync(string? staffType = null);
        Task<StaffResponseDto?> GetStaffByIdAsync(int id);
        Task<StaffResponseDto?> GetStaffByEmployeeIdAsync(string employeeId);
        Task<string> GetNextEmployeeIdAsync(string staffType);
        Task<StaffResponseDto> CreateStaffAsync(CreateStaffDto dto);
        Task<StaffResponseDto> UpdateStaffAsync(int id, UpdateStaffDto dto);
        Task<bool> DeleteStaffAsync(int id);

        Task<StaffResponseDto> UploadPhotoAsync(UploadStaffPhotoDto dto);
        Task<(string PhysicalPath, string ContentType)> GetPhotoAsync(int id);

        Task<StaffSubjectAllocationResponseDto> AssignSubjectAsync(AssignStaffSubjectDto dto);
        Task<StaffSubjectAllocationResponseDto> UpdateSubjectAllocationAsync(int id, UpdateStaffSubjectAllocationDto dto);
        Task<bool> DeleteSubjectAllocationAsync(int id);
        Task<List<StaffSubjectAllocationResponseDto>> GetStaffSubjectAllocationsAsync(int staffId);
        Task<StaffWorkloadResponseDto?> GetStaffWorkloadAsync(int staffId);
    }
}
