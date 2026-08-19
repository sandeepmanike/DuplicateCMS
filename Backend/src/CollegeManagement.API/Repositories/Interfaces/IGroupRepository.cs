using CollegeManagement.API.DTOs.Groups;

namespace CollegeManagement.API.Repositories
{
    public interface IGroupRepository
    {
        Task<List<GroupListItemDto>> GetAllAsync(string? search, int? boardId, int? academicYearId, int? academicLevelId, bool? isActive);
        Task<GroupResponse?> GetByIdAsync(int groupId);
        Task<List<GroupListItemDto>> GetByBoardAsync(int boardId);
        Task<GroupResponse> CreateAsync(CreateGroupRequest request);
        Task<GroupResponse?> UpdateAsync(int groupId, UpdateGroupRequest request);
        Task<bool> DeleteAsync(int groupId);
        Task<bool> ActivateAsync(int groupId, bool isActive = true);
        Task<bool> GroupCodeExistsAsync(string groupCode, int? excludeGroupId = null);
        Task<List<CollegeManagement.API.DTOs.Students.StudentListItemDto>> GetStudentsAsync(int groupId);
        Task<List<CollegeManagement.API.Models.Subject>> GetSubjectsAsync(int groupId);
        Task<GroupSummaryDto?> GetSummaryAsync(int groupId);
        Task<List<GroupDropdownDto>> GetDropdownAsync();
    }
}
