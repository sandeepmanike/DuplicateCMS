using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Versioning;
using CollegeManagement.API.DTOs.Staff;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/staff")]
    [Authorize]
    [Produces("application/json")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;

        public StaffController(IStaffService staffService)
        {
            _staffService = staffService;
        }

        /// <summary>
        /// 1. GET /api/v1/staff or /api/v1/faculty
        /// Get paged, searched, filtered (by StaffType, Department, Designation, Status) list of staff.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<StaffResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStaff([FromQuery] StaffQueryParams queryParams)
        {
            var result = await _staffService.GetPagedStaffAsync(queryParams);
            return Ok(result);
        }

        /// <summary>
        /// 2. GET /api/v1/staff/next-employee-id?staffType=Teaching
        /// Generates the next sequential Employee ID (PJCTCH0001 / PJCNTCH0001).
        /// </summary>
        [HttpGet("next-employee-id")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNextEmployeeId([FromQuery] string? staffType = "Teaching", [FromQuery] string? facultyType = null)
        {
            var type = !string.IsNullOrWhiteSpace(facultyType) ? facultyType : (staffType ?? "Teaching");
            var nextId = await _staffService.GetNextEmployeeIdAsync(type);
            return Ok(new { nextEmployeeId = nextId, employeeId = nextId, staffType = type });
        }

        /// <summary>
        /// 3. GET /api/v1/staff/dropdown
        /// Get list of staff for dropdown selection with optional staffType filter.
        /// </summary>
        [HttpGet("dropdown")]
        [ProducesResponseType(typeof(IEnumerable<StaffDropdownDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStaffDropdown([FromQuery] string? staffType = null, [FromQuery] string? facultyType = null)
        {
            var type = !string.IsNullOrWhiteSpace(facultyType) ? facultyType : staffType;
            var result = await _staffService.GetStaffDropdownAsync(type);
            return Ok(result);
        }

        /// <summary>
        /// 4. GET /api/v1/staff/{id}
        /// Get detailed staff record by ID.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var result = await _staffService.GetStaffByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 5. POST /api/v1/staff
        /// Create a new staff member (Teaching or Non-Teaching).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto dto)
        {
            var result = await _staffService.CreateStaffAsync(dto);
            return CreatedAtAction(nameof(GetStaffById), new { id = result.Id }, result);
        }

        /// <summary>
        /// 6. PUT /api/v1/staff/{id}
        /// Update an existing staff member.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] UpdateStaffDto dto)
        {
            var result = await _staffService.UpdateStaffAsync(id, dto);
            return Ok(result);
        }

        /// <summary>
        /// 7. DELETE /api/v1/staff/{id}
        /// Soft delete a staff member record.
        /// </summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            await _staffService.DeleteStaffAsync(id);
            return NoContent();
        }

        /// <summary>
        /// 8. POST /api/v1/staff/upload-photo
        /// Upload or replace staff member photo.
        /// </summary>
        [HttpPost("upload-photo")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadPhoto([FromForm] UploadStaffPhotoDto dto)
        {
            var result = await _staffService.UploadPhotoAsync(dto);
            return Ok(result);
        }

        /// <summary>
        /// 9. GET /api/v1/staff/photo/{id}
        /// Stream staff member profile photo.
        /// </summary>
        [HttpGet("photo/{id:int}")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaffPhoto(int id)
        {
            var (physicalPath, contentType) = await _staffService.GetPhotoAsync(id);
            return PhysicalFile(physicalPath, contentType);
        }
    }
}
