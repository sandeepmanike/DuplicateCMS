using CollegeManagement.API.DTOs.Groups;
using CollegeManagement.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeManagement.API.Exceptions;
using MySqlConnector;

namespace CollegeManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/groups")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;
        public GroupsController(IGroupRepository groupRepository) => _groupRepository = groupRepository;

        [HttpGet]
        public async Task<IActionResult> GetGroups(
            [FromQuery] string? search = null,
            [FromQuery] int? boardId = null,
            [FromQuery] int? academicYearId = null,
            [FromQuery] int? academicLevelId = null,
            [FromQuery] bool? isActive = null)
            => Ok(await _groupRepository.GetAllAsync(search, boardId, academicYearId, academicLevelId, isActive));

        [HttpGet("dropdown")]
        public async Task<IActionResult> GetDropdown() => Ok(await _groupRepository.GetDropdownAsync());

        [HttpGet("{groupId:int}/students")]
        public async Task<IActionResult> GetStudents(int groupId)
        { if (groupId <= 0) throw new ValidationException("Valid GroupId is required"); return Ok(await _groupRepository.GetStudentsAsync(groupId)); }

        [HttpGet("{groupId:int}/subjects")]
        public async Task<IActionResult> GetSubjects(int groupId)
        { if (groupId <= 0) throw new ValidationException("Valid GroupId is required"); return Ok(await _groupRepository.GetSubjectsAsync(groupId)); }

        [HttpGet("{groupId:int}/summary")]
        public async Task<IActionResult> GetSummary(int groupId)
        { if (groupId <= 0) throw new ValidationException("Valid GroupId is required"); var result = await _groupRepository.GetSummaryAsync(groupId); return result == null ? NotFound(new { message = "Group not found" }) : Ok(result); }

        [HttpGet("{groupId:int}")]
        public async Task<IActionResult> GetGroup(int groupId)
        { if (groupId <= 0) throw new ValidationException("Valid GroupId is required"); var group = await _groupRepository.GetByIdAsync(groupId); if (group == null) throw new NotFoundException("Group not found"); return Ok(group); }

        [HttpGet("board/{boardId:int}")]
        public async Task<IActionResult> GetGroupsByBoard(int boardId)
        { if (boardId <= 0) throw new ValidationException("Valid BoardId is required"); return Ok(await _groupRepository.GetByBoardAsync(boardId)); }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            try { var group = await _groupRepository.CreateAsync(request); return CreatedAtAction(nameof(GetGroup), new { groupId = group.GroupId }, new { message = "Group created successfully", data = group }); }
            catch (MySqlException ex) { HandleException(ex); throw; }
        }

        [HttpPut("{groupId:int}")]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] UpdateGroupRequest request)
        {
            if (groupId <= 0) throw new ValidationException("Valid GroupId is required");
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            try { var group = await _groupRepository.UpdateAsync(groupId, request); if (group == null) throw new NotFoundException("Group not found"); return Ok(new { message = "Group updated successfully", data = group }); }
            catch (MySqlException ex) { HandleException(ex); throw; }
        }

        [HttpDelete("{groupId:int}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            if (groupId <= 0) throw new ValidationException("Valid GroupId is required");
            try { var deleted = await _groupRepository.DeleteAsync(groupId); if (!deleted) throw new NotFoundException("Group not found"); return Ok(new { message = "Group deleted successfully" }); }
            catch (MySqlException ex) { HandleException(ex); throw; }
        }

        [HttpPatch("{groupId:int}/activate")]
        public async Task<IActionResult> ActivateGroup(int groupId, [FromQuery] bool isActive = true)
        {
            if (groupId <= 0) throw new ValidationException("Valid GroupId is required");
            var success = await _groupRepository.ActivateAsync(groupId, isActive);
            if (!success) throw new NotFoundException("Group not found");
            return Ok(new { message = isActive ? "Group activated successfully" : "Group deactivated successfully", data = new { groupId, isActive } });
        }

        [HttpGet("validate-code")]
        public async Task<IActionResult> ValidateGroupCode([FromQuery] string groupCode, [FromQuery] int? excludeGroupId = null)
        {
            if (string.IsNullOrWhiteSpace(groupCode)) throw new ValidationException("Group code is required");
            var exists = await _groupRepository.GroupCodeExistsAsync(groupCode, excludeGroupId);
            return Ok(new { groupCode, exists, isAvailable = !exists });
        }

        private static void HandleException(MySqlException exception)
        {
            var message = exception.Message;
            if (message.Contains("already exists", StringComparison.OrdinalIgnoreCase)) throw new ConflictException(message);
            if (message.Contains("not found", StringComparison.OrdinalIgnoreCase)) throw new NotFoundException(message);
            throw new ValidationException(message);
        }
    }
}
