using CollegeManagement.API.DTOs;
using CollegeManagement.API.DTOs.Subject;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers
{
    /// <summary>
    /// API controller for Subject management, handling creation, retrieval, updates, and deletion of subjects.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _service;

        public SubjectsController(ISubjectService service)
        {
            _service = service;
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? search = null,
            [FromQuery] int? boardId = null,
            [FromQuery] int? academicYearId = null,
            [FromQuery] int? groupId = null,
            [FromQuery] bool? isActive = null)
            => Ok(await _service.SearchAsync(search, boardId, academicYearId, groupId, isActive));

        [HttpGet("active")]
        public async Task<IActionResult> GetActive() => Ok(await _service.GetActiveAsync());

        [HttpGet("board/{boardId:int}")]
        public async Task<IActionResult> GetByBoard(int boardId)
        {
            if (boardId <= 0) return BadRequest(new { message = "Valid BoardId is required." });
            return Ok(await _service.GetByBoardIdAsync(boardId));
        }

        [HttpGet("academic-year/{academicYearId:int}")]
        public async Task<IActionResult> GetByAcademicYear(int academicYearId)
        {
            if (academicYearId <= 0) return BadRequest(new { message = "Valid AcademicYearId is required." });
            return Ok(await _service.GetByAcademicYearIdAsync(academicYearId));
        }

        [HttpGet("check-code")]
        public async Task<IActionResult> CheckCode([FromQuery] string subjectCode, [FromQuery] int? excludeSubjectId = null)
        {
            if (string.IsNullOrWhiteSpace(subjectCode)) return BadRequest(new { message = "Subject code is required." });
            var exists = await _service.SubjectCodeExistsAsync(subjectCode, excludeSubjectId);
            return Ok(new { subjectCode, exists, isAvailable = !exists });
        }

        // ==========================
        // GET ALL SUBJECTS
        // ==========================
        /// <summary>
        /// Retrieves all subjects.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _service.GetAllAsync();
            return Ok(subjects);
        }

        // ==========================
        // GET SUBJECT BY ID
        // ==========================
        /// <summary>
        /// Retrieves a specific subject by its identifier.
        /// </summary>
        /// <param name="id">The subject identifier.</param>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            var subject = await _service.GetByIdAsync(id);

            if (subject == null)
                return NotFound(new
                {
                    message = "Subject not found."
                });

            return Ok(subject);
        }

        // ==========================
        // GET SUBJECTS BY GROUP
        // ==========================
        /// <summary>
        /// Retrieves subjects associated with a specific academic group name.
        /// </summary>
        /// <param name="group">The group name.</param>
        [HttpGet("group/{groupId:int}")]
        public async Task<IActionResult> GetSubjectsByGroupId(int groupId)
        {
            if (groupId <= 0)
                return BadRequest(new { message = "Valid GroupId is required." });

            return Ok(await _service.GetByGroupIdAsync(groupId));
        }

        // ==========================
        // CREATE SUBJECT
        // ==========================
        /// <summary>
        /// Creates a new subject.
        /// </summary>
        /// <param name="dto">The subject details to create.</param>
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

        // ==========================
        // UPDATE SUBJECT
        // ==========================
        /// <summary>
        /// Updates an existing subject.
        /// </summary>
        /// <param name="id">The subject identifier to update.</param>
        /// <param name="dto">The updated subject details.</param>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);
            var subject = await _service.UpdateAsync(id, dto);

            if (subject == null)
            {
                return NotFound(new
                {
                    message = "Subject not found."
                });
            }

            return Ok(subject);
        }

        // ==========================
        // DELETE SUBJECT
        // ==========================
        /// <summary>
        /// Deletes a specific subject by its identifier.
        /// </summary>
        /// <param name="id">The subject identifier to delete.</param>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Subject not found."
                });
            }

            return Ok(new
            {
                message = "Subject deleted successfully."
            });
        }
    }
}