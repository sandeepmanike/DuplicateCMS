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
        [HttpGet("group/{group}")]
        public async Task<IActionResult> GetSubjectsByGroup(string group)
        {
            var subjects = await _service.GetByGroupAsync(group);

            return Ok(subjects);
        }

        // ==========================
        // CREATE SUBJECT
        // ==========================
        /// <summary>
        /// Creates a new subject.
        /// </summary>
        /// <param name="dto">The subject details to create.</param>
        [HttpPost]
        public async Task<IActionResult> CreateSubject(CreateSubjectDto dto)
        {
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
        public async Task<IActionResult> UpdateSubject(int id, UpdateSubjectDto dto)
        {
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