using Asp.Versioning;
using CollegeManagement.API.DTOs.Students;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/students")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;

        public StudentsController(IStudentService service)
        {
            _service = service;
        }

        // =========================================================
        // GET ALL STUDENTS
        // =========================================================
        [HttpGet]
        [ProducesResponseType(typeof(List<StudentListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // =========================================================
        // GET STUDENT BY ID
        // =========================================================
        [HttpGet("{studentId:int}")]
        [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int studentId)
        {
            var student = await _service.GetByIdAsync(studentId);
            if (student == null)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            return Ok(student);
        }

        // =========================================================
        // UPDATE STUDENT (ADMIN FULL EDIT)
        // =========================================================
        [HttpPut("{studentId:int}")]
        [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int studentId, [FromBody] UpdateStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateAsync(studentId, request);
            if (updated == null)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            return Ok(updated);
        }

        // =========================================================
        // DELETE STUDENT (SOFT DELETE)
        // =========================================================
        [HttpDelete("{studentId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int studentId)
        {
            var success = await _service.DeleteAsync(studentId);
            if (!success)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            return Ok(new { message = "Student deactivated successfully." });
        }

        // =========================================================
        // GET STUDENT 360 PROFILE
        // =========================================================
        [HttpGet("{studentId:int}/profile")]
        [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfile(int studentId)
        {
            var profile = await _service.GetProfileAsync(studentId);
            if (profile == null)
                return NotFound(new { message = $"Student profile for ID {studentId} not found." });

            return Ok(profile);
        }

        // =========================================================
        // UPDATE STUDENT PROFILE
        // =========================================================
        [HttpPut("{studentId:int}/profile")]
        [ProducesResponseType(typeof(StudentProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateProfile(int studentId, [FromBody] UpdateStudentProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdateProfileAsync(studentId, request);
            if (updated == null)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            return Ok(updated);
        }

        // =========================================================
        // CHANGE SECTION
        // =========================================================
        [HttpPatch("{studentId:int}/section")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeSection(int studentId, [FromBody] ChangeSectionRequest request)
        {
            if (request.SectionId <= 0)
                return BadRequest(new { message = "Valid SectionId is required." });

            var success = await _service.ChangeSectionAsync(studentId, request);
            if (!success)
                return NotFound(new { message = $"Student with ID {studentId} not found or invalid section." });

            return Ok(new { message = "Student section changed successfully." });
        }

        // =========================================================
        // CHANGE GROUP (REQUIRES TARGET GROUP + SECTION)
        // =========================================================
        [HttpPatch("{studentId:int}/group")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeGroup(int studentId, [FromBody] ChangeGroupRequest request)
        {
            if (request.GroupId <= 0 || request.SectionId <= 0)
                return BadRequest(new { message = "Valid GroupId and SectionId are required." });

            var success = await _service.ChangeGroupAsync(studentId, request);
            if (!success)
                return NotFound(new { message = $"Student with ID {studentId} not found or invalid group/section." });

            return Ok(new { message = "Student group and section updated successfully." });
        }

        // =========================================================
        // TRANSFER STUDENT (FULL ACADEMIC TRANSFER)
        // =========================================================
        [HttpPost("{studentId:int}/transfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Transfer(int studentId, [FromBody] TransferStudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var success = await _service.TransferAsync(studentId, request);
            if (!success)
                return NotFound(new { message = $"Student with ID {studentId} not found or transfer failed." });

            return Ok(new { message = "Student transferred successfully." });
        }

        // =========================================================
        // SUSPEND STUDENT
        // =========================================================
        [HttpPatch("{studentId:int}/suspend")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Suspend(int studentId, [FromBody] SuspendStudentRequest request)
        {
            var success = await _service.SuspendAsync(studentId, request);
            if (!success)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            return Ok(new { message = "Student suspended successfully." });
        }

        // =========================================================
        // ACTIVATE STUDENT
        // =========================================================
        [HttpPatch("{studentId:int}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Activate(int studentId)
        {
            var success = await _service.ActivateAsync(studentId);
            if (!success)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            return Ok(new { message = "Student activated successfully." });
        }

        // =========================================================
        // RESET PASSWORD
        // =========================================================
        [HttpPost("{studentId:int}/reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ResetPassword(int studentId)
        {
            var success = await _service.ResetPasswordAsync(studentId);
            if (!success)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            return Ok(new { message = "Student password reset successfully." });
        }

        // =========================================================
        // STUDENT DASHBOARD
        // =========================================================
        [HttpGet("{studentId:int}/dashboard")]
        [ProducesResponseType(typeof(StudentDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDashboard(int studentId)
        {
            var dashboard = await _service.GetDashboardAsync(studentId);
            if (dashboard == null)
                return NotFound(new { message = $"Student with ID {studentId} not found." });

            return Ok(dashboard);
        }

        // =========================================================
        // SEARCH STUDENTS (WITH BOARD, YEAR, LEVEL, GROUP, SECTION)
        // =========================================================
        [HttpGet("search")]
        [ProducesResponseType(typeof(List<StudentListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(
            [FromQuery] string? search,
            [FromQuery] int? boardId,
            [FromQuery] int? academicYearId,
            [FromQuery] int? academicLevelId,
            [FromQuery] int? groupId,
            [FromQuery] int? sectionId,
            [FromQuery] bool? isActive)
        {
            var result = await _service.SearchAsync(
                search,
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                sectionId,
                isActive);

            return Ok(result);
        }

        // =========================================================
        // GET ACTIVE STUDENTS
        // =========================================================
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<StudentListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive()
        {
            var result = await _service.GetActiveAsync();
            return Ok(result);
        }

        // =========================================================
        // GET STUDENTS BY GROUP
        // =========================================================
        [HttpGet("group/{groupId:int}")]
        [ProducesResponseType(typeof(List<StudentListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByGroup(int groupId)
        {
            var result = await _service.GetByGroupAsync(groupId);
            return Ok(result);
        }

        // =========================================================
        // GET STUDENTS BY SECTION
        // =========================================================
        [HttpGet("section/{sectionId:int}")]
        [ProducesResponseType(typeof(List<StudentListItemDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBySection(int sectionId)
        {
            var result = await _service.GetBySectionAsync(sectionId);
            return Ok(result);
        }

        // =========================================================
        // CHECK EMAIL
        // =========================================================
        [HttpGet("check-email")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckEmail([FromQuery] string email, [FromQuery] int? excludeStudentId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest(new { message = "Email query parameter is required." });

            var exists = await _service.EmailExistsAsync(email, excludeStudentId);
            return Ok(new { exists });
        }

        // =========================================================
        // CHECK MOBILE
        // =========================================================
        [HttpGet("check-mobile")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckMobile([FromQuery] string mobileNumber, [FromQuery] int? excludeStudentId = null)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
                return BadRequest(new { message = "Mobile number query parameter is required." });

            var exists = await _service.MobileExistsAsync(mobileNumber, excludeStudentId);
            return Ok(new { exists });
        }
    }
}
