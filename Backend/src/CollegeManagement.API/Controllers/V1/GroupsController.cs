using CollegeManagement.API.DTOs.Groups;
using CollegeManagement.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CollegeManagement.API.Exceptions;
using MySqlConnector;

namespace CollegeManagement.API.Controllers
{
    /// <summary>
    /// API controller for Group management, handling creation, retrieval, updates, and deletion of academic groups.
    /// </summary>
    [ApiController]
    [Route("api/v1/groups")]
    [Authorize]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupRepository _groupRepository;

        public GroupsController(
            IGroupRepository groupRepository)
        {
            _groupRepository = groupRepository;
        }

        /// <summary>
        /// Retrieves a paginated, filtered, and searched list of academic groups.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetGroups(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? search = null,
            [FromQuery] string? board = null,
            [FromQuery] int? academicYearId = null,
            [FromQuery] string? academicLevel = null,
            [FromQuery] bool? isActive = null)
        {
            if (pageNumber < 1)
            {
                pageNumber = 1;
            }

            if (pageSize < 1 || pageSize > 100)
            {
                pageSize = 20;
            }

            var result = await _groupRepository.GetAllAsync(
                pageNumber,
                pageSize,
                search,
                board,
                academicYearId,
                academicLevel,
                isActive);

            return Ok(result);
        }

        /// <summary>
        /// Retrieves detailed information for a specific academic group by ID.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        [HttpGet("{groupId:int}")]
        public async Task<IActionResult> GetGroup(
            int groupId)
        {
            if (groupId <= 0)
            {
                throw new ValidationException("Valid GroupId is required");
            }

            var group =
                await _groupRepository.GetByIdAsync(groupId);

            if (group == null)
            {
                throw new NotFoundException("Group not found");
            }

            return Ok(group);
        }

        /// <summary>
        /// Retrieves academic groups filtered by board name.
        /// </summary>
        /// <param name="board">The board name.</param>
        [HttpGet("board/{board}")]
        public async Task<IActionResult> GetGroupsByBoard(
            string board)
        {
            if (string.IsNullOrWhiteSpace(board))
            {
                throw new ValidationException("Board is required");
            }

            var groups =
                await _groupRepository.GetByBoardAsync(board);

            return Ok(groups);
        }

        /// <summary>
        /// Creates a new academic group.
        /// </summary>
        /// <param name="request">The group details to create.</param>
        [HttpPost]
        public async Task<IActionResult> CreateGroup(
            [FromBody] CreateGroupRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var group =
                    await _groupRepository.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetGroup),
                    new
                    {
                        groupId = group.GroupId
                    },
                    new
                    {
                        message = "Group created successfully",
                        data = group
                    });
            }
            catch (MySqlException ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Updates an existing academic group.
        /// </summary>
        /// <param name="groupId">The group identifier to update.</param>
        /// <param name="request">The updated group configuration values.</param>
        [HttpPut("{groupId:int}")]
        public async Task<IActionResult> UpdateGroup(
            int groupId,
            [FromBody] UpdateGroupRequest request)
        {
            if (groupId <= 0)
            {
                throw new ValidationException("Valid GroupId is required");
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var group =
                    await _groupRepository.UpdateAsync(
                        groupId,
                        request);

                if (group == null)
                {
                    throw new NotFoundException("Group not found");
                }

                return Ok(new
                {
                    message = "Group updated successfully",
                    data = group
                });
            }
            catch (MySqlException ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Deletes a specific academic group by ID.
        /// </summary>
        /// <param name="groupId">The group identifier to delete.</param>
        [HttpDelete("{groupId:int}")]
        public async Task<IActionResult> DeleteGroup(
            int groupId)
        {
            if (groupId <= 0)
            {
                throw new ValidationException("Valid GroupId is required");
            }

            try
            {
                var deleted =
                    await _groupRepository.DeleteAsync(groupId);

                if (!deleted)
                {
                    throw new NotFoundException("Group not found");
                }

                return Ok(new
                {
                    message = "Group deleted successfully"
                });
            }
            catch (MySqlException ex)
            {
                HandleException(ex);
                throw;
            }
        }

        /// <summary>
        /// Validates if a group code is available for use.
        /// </summary>
        /// <param name="groupCode">The group code to validate.</param>
        /// <param name="excludeGroupId">An optional group ID to exclude from validation.</param>
        [HttpGet("validate-code")]
        public async Task<IActionResult> ValidateGroupCode(
            [FromQuery] string groupCode,
            [FromQuery] int? excludeGroupId = null)
        {
            if (string.IsNullOrWhiteSpace(groupCode))
            {
                throw new ValidationException("Group code is required");
            }

            var exists =
                await _groupRepository.GroupCodeExistsAsync(
                    groupCode,
                    excludeGroupId);

            return Ok(new
            {
                groupCode,
                exists,
                isAvailable = !exists
            });
        }

        private static void HandleException(MySqlException exception)
        {
            var message = exception.Message;
            if (message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
                throw new ConflictException(message);
            }
            if (message.Contains("not found", StringComparison.OrdinalIgnoreCase))
            {
                throw new NotFoundException(message);
            }
            throw new ValidationException(message);
        }
    }
}