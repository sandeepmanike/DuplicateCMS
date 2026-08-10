using Asp.Versioning;
using CollegeManagement.API.DTOs.Students;
using CollegeManagement.API.Services;

using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/students")]
 
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;
        public StudentsController(IStudentService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{studentId:int}")]
        public async Task<IActionResult> GetById(int studentId)
        {
            var r = await _service.GetByIdAsync(studentId);
            return r == null ? NotFound(new { Message = "Student not found." }) : Ok(r);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var r = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { studentId = r.StudentId }, r);
        }

        [HttpPut("{studentId:int}")]
        public async Task<IActionResult> Update(int studentId, UpdateStudentRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var r = await _service.UpdateAsync(studentId, request);
            return r == null ? NotFound(new { Message = "Student not found." }) : Ok(r);
        }

        [HttpDelete("{studentId:int}")]
        public async Task<IActionResult> Delete(int studentId)
        {
            var ok = await _service.DeleteAsync(studentId);
            return ok ? Ok(new { Message = "Student deleted successfully." }) : NotFound(new { Message = "Student not found." });
        }

        [HttpGet("{studentId:int}/profile")]
        public async Task<IActionResult> GetProfile(int studentId)
        {
            var r = await _service.GetProfileAsync(studentId);
            return r == null ? NotFound(new { Message = "Student profile not found." }) : Ok(r);
        }

        [HttpPut("{studentId:int}/profile")]
        public async Task<IActionResult> UpdateProfile(int studentId, StudentProfileDto request)
        {
            var r = await _service.UpdateProfileAsync(studentId, request);
            return r == null ? NotFound(new { Message = "Student profile not found." }) : Ok(r);
        }

        [HttpPatch("{studentId:int}/section")]
        public async Task<IActionResult> ChangeSection(int studentId, ChangeSectionRequest request) => Ok(new { Success = await _service.ChangeSectionAsync(studentId, request), Message = "Student section updated successfully." });

        [HttpPatch("{studentId:int}/group")]
        public async Task<IActionResult> ChangeGroup(int studentId, ChangeGroupRequest request) => Ok(new { Success = await _service.ChangeGroupAsync(studentId, request), Message = "Student group updated successfully." });

        [HttpPost("{studentId:int}/transfer")]
        public async Task<IActionResult> Transfer(int studentId, TransferStudentRequest request) => Ok(new { Success = await _service.TransferAsync(studentId, request), Message = "Student transferred successfully." });

        [HttpPatch("{studentId:int}/suspend")]
        public async Task<IActionResult> Suspend(int studentId, SuspendStudentRequest request) => Ok(new { Success = await _service.SuspendAsync(studentId, request), Message = "Student suspended successfully." });

        [HttpPatch("{studentId:int}/activate")]
        public async Task<IActionResult> Activate(int studentId) => Ok(new { Success = await _service.ActivateAsync(studentId), Message = "Student activated successfully." });

        [HttpPost("{studentId:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(int studentId) => Ok(new { Success = await _service.ResetPasswordAsync(studentId), Message = "Student password reset successfully." });

        [HttpGet("{studentId:int}/dashboard")]
        public async Task<IActionResult> GetDashboard(int studentId)
        {
            var r = await _service.GetDashboardAsync(studentId);
            return r == null ? NotFound(new { Message = "Student dashboard not found." }) : Ok(r);
        }
    }
}