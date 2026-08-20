using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Subject;
using CollegeManagement.API.Models;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers
{
    /// <summary>
    /// API controller for Subject management, handling creation, retrieval, updates, and deletion of subjects by academic context (Board + Group + Academic Level).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _service;

        public SubjectsController(ISubjectService service)
        {
            _service = service;
        }

        /// <summary>
        /// Retrieves all subjects, optionally filtered by BoardId, GroupId, and AcademicLevelId.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSubjects(
            [FromQuery] int? boardId = null,
            [FromQuery] int? groupId = null,
            [FromQuery] int? academicLevelId = null)
        {
            if (boardId.HasValue && groupId.HasValue && academicLevelId.HasValue)
            {
                var contextSubjects = await _service.GetByContextAsync(boardId.Value, groupId.Value, academicLevelId.Value);
                return Ok(contextSubjects);
            }

            var subjects = await _service.GetAllAsync();
            return Ok(subjects);
        }

        /// <summary>
        /// Searches subjects with filters for search keyword, BoardId, GroupId, AcademicLevelId, and active status.
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? search = null,
            [FromQuery] int? boardId = null,
            [FromQuery] int? groupId = null,
            [FromQuery] int? academicLevelId = null,
            [FromQuery] bool? isActive = null)
        {
            var results = await _service.SearchAsync(search, boardId, groupId, academicLevelId, isActive);
            return Ok(results);
        }

        /// <summary>
        /// Retrieves subjects for a specific academic context (Board + Group + Academic Level).
        /// </summary>
        [HttpGet("context")]
        public async Task<IActionResult> GetByContext(
            [FromQuery] int boardId,
            [FromQuery] int groupId,
            [FromQuery] int academicLevelId)
        {
            if (boardId <= 0 || groupId <= 0 || academicLevelId <= 0)
                return BadRequest(new { message = "Valid BoardId, GroupId, and AcademicLevelId are required." });

            var subjects = await _service.GetByContextAsync(boardId, groupId, academicLevelId);
            return Ok(subjects);
        }

        /// <summary>
        /// Checks if a subject code already exists in the given context.
        /// </summary>
        [HttpGet("check-code")]
        public async Task<IActionResult> CheckCode(
            [FromQuery] string subjectCode,
            [FromQuery] int boardId = 0,
            [FromQuery] int groupId = 0,
            [FromQuery] int academicLevelId = 0,
            [FromQuery] int? excludeSubjectId = null)
        {
            if (string.IsNullOrWhiteSpace(subjectCode))
                return BadRequest(new { message = "Subject code is required." });

            var exists = await _service.SubjectCodeExistsAsync(subjectCode, boardId, groupId, academicLevelId, excludeSubjectId);
            return Ok(new { subjectCode, exists, isAvailable = !exists });
        }

        /// <summary>
        /// Retrieves all active subjects.
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive() => Ok(await _service.GetActiveAsync());

        /// <summary>
        /// Retrieves subjects associated with a specific board.
        /// </summary>
        [HttpGet("board/{boardId:int}")]
        public async Task<IActionResult> GetByBoard(int boardId)
        {
            if (boardId <= 0) return BadRequest(new { message = "Valid BoardId is required." });
            return Ok(await _service.GetByBoardIdAsync(boardId));
        }

        /// <summary>
        /// Retrieves a specific subject by its identifier.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _service.GetByIdAsync(id);
            if (subject == null)
                return NotFound(new { message = "Subject not found." });

            return Ok(subject);
        }

        /// <summary>
        /// Retrieves subjects associated with a specific academic group identifier.
        /// </summary>
        [HttpGet("group/{groupId:int}")]
        public async Task<IActionResult> GetSubjectsByGroupId(int groupId)
        {
            if (groupId <= 0)
                return BadRequest(new { message = "Valid GroupId is required." });

            return Ok(await _service.GetByGroupIdAsync(groupId));
        }

        /// <summary>
        /// Creates a new subject in the specified academic context (Board + Group + Academic Level).
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var subject = await _service.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetSubjectById),
                new { id = subject.SubjectId },
                subject);
        }

        /// <summary>
        /// Updates an existing subject in the specified academic context.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var subject = await _service.UpdateAsync(id, dto);

            if (subject == null)
                return NotFound(new { message = "Subject not found." });

            return Ok(subject);
        }

        /// <summary>
        /// Deletes/deactivates a specific subject by its identifier.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Subject not found." });

            return Ok(new { message = "Subject deleted successfully." });
        }
    }
}
