using CollegeManagement.API.DTOs.StudentAdmission;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/admissions")]
    public class StudentAdmissionsController : ControllerBase
    {
        private readonly IStudentAdmissionService _service;

        public StudentAdmissionsController(
            IStudentAdmissionService service)
        {
            _service = service;
        }

        // GET: api/v1/admissions
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        // GET: api/v1/admissions/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    Message = "Admission not found."
                });
            }

            return Ok(result);
        }

        // POST: api/v1/admissions
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
            [FromForm] CreateStudentAdmissionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = result.AdmissionId },
                result);
        }

        // PUT: api/v1/admissions/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] UpdateStudentAdmissionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateAsync(id, request);

            if (result == null)
            {
                return NotFound(new
                {
                    Message = "Admission not found."
                });
            }

            return Ok(result);
        }

        // DELETE: api/v1/admissions/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    Message = "Admission not found."
                });
            }

            return Ok(new
            {
                Message = "Admission deleted successfully."
            });
        }

        // POST: api/v1/admissions/5/verify
        [HttpPost("{id:int}/verify")]
        public async Task<IActionResult> Verify(int id)
        {
            var result = await _service.VerifyAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Message = "Admission not found."
                });
            }

            return Ok(new
            {
                Message = "Admission verified successfully."
            });
        }

        // POST: api/v1/admissions/5/approve
        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _service.ApproveAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Message = "Admission not found."
                });
            }

            return Ok(new
            {
                Message = "Admission approved successfully."
            });
        }

        // POST: api/v1/admissions/5/reject
        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var result = await _service.RejectAsync(id);

            if (!result)
            {
                return NotFound(new
                {
                    Message = "Admission not found."
                });
            }

            return Ok(new
            {
                Message = "Admission rejected successfully."
            });
        }

        // POST: api/v1/admissions/generate-number
        [HttpPost("generate-number")]
        public async Task<IActionResult> GenerateAdmissionNumber()
        {
            var admissionNo = await _service.GenerateAdmissionNumberAsync();

            return Ok(new
            {
                AdmissionNumber = admissionNo
            });
        }
    }
}