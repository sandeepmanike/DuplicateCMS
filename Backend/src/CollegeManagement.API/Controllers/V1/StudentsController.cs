using CollegeManagement.API.DTOs.Students;
using CollegeManagement.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1
{
    [ApiController]
    [Route("api/v1/students")]
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
        public async Task<IActionResult> GetAll()
        {
            var students = await _service.GetAllAsync();

            return Ok(students);
        }


        // =========================================================
        // GET STUDENT BY ID
        // =========================================================

        [HttpGet("{studentId:int}")]
        public async Task<IActionResult> GetById(int studentId)
        {
            var student = await _service.GetByIdAsync(studentId);

            if (student == null)
                return NotFound(new
                {
                    message = "Student not found"
                });

            return Ok(student);
        }


        // =========================================================
        // CREATE STUDENT
        // =========================================================

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateStudentRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _service.CreateAsync(request);

            return Ok(result);
        }


        // =========================================================
        // UPDATE STUDENT
        // =========================================================

        [HttpPut("{studentId:int}")]
        public async Task<IActionResult> Update(
            int studentId,
            [FromBody] UpdateStudentRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _service.UpdateAsync(
                studentId,
                request);

            if (result == null)
                return NotFound(new
                {
                    message = "Student not found"
                });

            return Ok(result);
        }


        // =========================================================
        // DELETE STUDENT
        // =========================================================

        [HttpDelete("{studentId:int}")]
        public async Task<IActionResult> Delete(
            int studentId)
        {
            var result = await _service.DeleteAsync(studentId);

            if (!result)
                return NotFound(new
                {
                    message = "Student not found"
                });

            return Ok(new
            {
                message = "Student deleted successfully"
            });
        }


        // =========================================================
        // GET STUDENT PROFILE
        // =========================================================

        [HttpGet("{studentId:int}/profile")]
        public async Task<IActionResult> GetProfile(
            int studentId)
        {
            var profile = await _service.GetProfileAsync(
                studentId);

            if (profile == null)
                return NotFound(new
                {
                    message = "Student not found"
                });

            return Ok(profile);
        }


        // =========================================================
        // UPDATE STUDENT PROFILE
        // =========================================================

        [HttpPut("{studentId:int}/profile")]
        public async Task<IActionResult> UpdateProfile(
            int studentId,
            [FromBody] StudentProfileDto request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var profile = await _service.UpdateProfileAsync(
                studentId,
                request);

            if (profile == null)
                return NotFound(new
                {
                    message = "Student not found"
                });

            return Ok(profile);
        }


        // =========================================================
        // CHANGE SECTION
        // =========================================================

        [HttpPut("{studentId:int}/section")]
        public async Task<IActionResult> ChangeSection(
            int studentId,
            [FromBody] ChangeSectionRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _service.ChangeSectionAsync(
                studentId,
                request);

            if (!result)
                return BadRequest(new
                {
                    message = "Section update failed"
                });

            return Ok(new
            {
                message = "Student section updated successfully"
            });
        }


        // =========================================================
        // CHANGE GROUP
        // =========================================================

        [HttpPut("{studentId:int}/group")]
        public async Task<IActionResult> ChangeGroup(
            int studentId,
            [FromBody] ChangeGroupRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _service.ChangeGroupAsync(
                studentId,
                request);

            if (!result)
                return BadRequest(new
                {
                    message = "Group update failed"
                });

            return Ok(new
            {
                message = "Student group updated successfully"
            });
        }


        // =========================================================
        // TRANSFER STUDENT
        // =========================================================

        [HttpPost("{studentId:int}/transfer")]
        public async Task<IActionResult> Transfer(
            int studentId,
            [FromBody] TransferStudentRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _service.TransferAsync(
                studentId,
                request);

            if (!result)
                return BadRequest(new
                {
                    message = "Student transfer failed"
                });

            return Ok(new
            {
                message = "Student transferred successfully"
            });
        }


        // =========================================================
        // SUSPEND STUDENT
        // =========================================================

        [HttpPost("{studentId:int}/suspend")]
        public async Task<IActionResult> Suspend(
            int studentId,
            [FromBody] SuspendStudentRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            var result = await _service.SuspendAsync(
                studentId,
                request);

            if (!result)
                return BadRequest(new
                {
                    message = "Student suspension failed"
                });

            return Ok(new
            {
                message = "Student suspended successfully"
            });
        }


        // =========================================================
        // ACTIVATE STUDENT
        // =========================================================

        [HttpPost("{studentId:int}/activate")]
        public async Task<IActionResult> Activate(
            int studentId)
        {
            var result = await _service.ActivateAsync(
                studentId);

            if (!result)
                return BadRequest(new
                {
                    message = "Student activation failed"
                });

            return Ok(new
            {
                message = "Student activated successfully"
            });
        }


        // =========================================================
        // RESET PASSWORD
        // =========================================================

        [HttpPost("{studentId:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            int studentId)
        {
            var result = await _service.ResetPasswordAsync(
                studentId);

            if (!result)
                return BadRequest(new
                {
                    message = "Password reset failed"
                });

            return Ok(new
            {
                message = "Student password reset successfully"
            });
        }


        // =========================================================
        // STUDENT DASHBOARD
        // =========================================================

        [HttpGet("{studentId:int}/dashboard")]
        public async Task<IActionResult> GetDashboard(
            int studentId)
        {
            var dashboard = await _service.GetDashboardAsync(
                studentId);

            if (dashboard == null)
                return NotFound(new
                {
                    message = "Student not found"
                });

            return Ok(dashboard);
        }


        // =========================================================
        // SEARCH STUDENTS
        // =========================================================

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? search,
            [FromQuery] int? boardId,
            [FromQuery] int? academicYearId,
            [FromQuery] int? academicLevelId,
            [FromQuery] int? groupId,
            [FromQuery] int? sectionId,
            [FromQuery] bool? isActive)
        {
            var students = await _service.SearchAsync(
                search,
                boardId,
                academicYearId,
                academicLevelId,
                groupId,
                sectionId,
                isActive);

            return Ok(students);
        }


        // =========================================================
        // GET STUDENTS BY GROUP
        // =========================================================

        [HttpGet("group/{groupId:int}")]
        public async Task<IActionResult> GetByGroup(
            int groupId)
        {
            var students = await _service.GetByGroupAsync(
                groupId);

            return Ok(students);
        }


        // =========================================================
        // GET STUDENTS BY SECTION
        // =========================================================

        [HttpGet("section/{sectionId:int}")]
        public async Task<IActionResult> GetBySection(
            int sectionId)
        {
            var students = await _service.GetBySectionAsync(
                sectionId);

            return Ok(students);
        }


        // =========================================================
        // GET ACTIVE STUDENTS
        // =========================================================

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var students = await _service.GetActiveAsync();

            return Ok(students);
        }


        // =========================================================
        // CHECK EMAIL
        // =========================================================

        [HttpGet("check-email")]
        public async Task<IActionResult> EmailExists(
            [FromQuery] string email,
            [FromQuery] int? excludeStudentId = null)
        {
            var exists = await _service.EmailExistsAsync(
                email,
                excludeStudentId);

            return Ok(new
            {
                exists
            });
        }


        // =========================================================
        // CHECK MOBILE
        // =========================================================

        [HttpGet("check-mobile")]
        public async Task<IActionResult> MobileExists(
            [FromQuery] string mobile,
            [FromQuery] int? excludeStudentId = null)
        {
            var exists = await _service.MobileExistsAsync(
                mobile,
                excludeStudentId);

            return Ok(new
            {
                exists
            });
        }
    }
}