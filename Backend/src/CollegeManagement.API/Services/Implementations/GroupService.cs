using CollegeManagement.API.DTOs.Groups;
using CollegeManagement.API.Repositories;

namespace CollegeManagement.API.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        public GroupService(IGroupRepository groupRepository) => _groupRepository = groupRepository;
        public Task<List<GroupListItemDto>> GetAllAsync(string? search, int? boardId, int? academicYearId, int? academicLevelId, bool? isActive) => _groupRepository.GetAllAsync(search, boardId, academicYearId, academicLevelId, isActive);
        public Task<GroupResponse?> GetByIdAsync(int groupId) => _groupRepository.GetByIdAsync(groupId);
        public Task<List<GroupListItemDto>> GetByBoardAsync(int boardId) => _groupRepository.GetByBoardAsync(boardId);
        public Task<GroupResponse> CreateAsync(CreateGroupRequest request) => _groupRepository.CreateAsync(request);
        public Task<GroupResponse?> UpdateAsync(int groupId, UpdateGroupRequest request) => _groupRepository.UpdateAsync(groupId, request);
        public Task<bool> DeleteAsync(int groupId) => _groupRepository.DeleteAsync(groupId);
        public Task<bool> ActivateAsync(int groupId, bool isActive = true) => _groupRepository.ActivateAsync(groupId, isActive);
        public Task<bool> GroupCodeExistsAsync(string groupCode, int? excludeGroupId = null) => _groupRepository.GroupCodeExistsAsync(groupCode, excludeGroupId);
        public Task<List<CollegeManagement.API.DTOs.Students.StudentListItemDto>> GetStudentsAsync(int groupId) => _groupRepository.GetStudentsAsync(groupId);
        public Task<List<CollegeManagement.API.Models.Subject>> GetSubjectsAsync(int groupId) => _groupRepository.GetSubjectsAsync(groupId);
        public Task<GroupSummaryDto?> GetSummaryAsync(int groupId) => _groupRepository.GetSummaryAsync(groupId);
        public Task<List<GroupDropdownDto>> GetDropdownAsync() => _groupRepository.GetDropdownAsync();
    }
}
