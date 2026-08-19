using CollegeManagement.API.DTOs.StudentAdmission;
using CollegeManagement.API.Services.Implementations;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CollegeManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/admissions")]
    public class StudentAdmissionsController : ControllerBase
    {
        private readonly IStudentAdmissionService _service;
        private readonly ILogger<StudentAdmissionsController> _logger;

        public StudentAdmissionsController(
            IStudentAdmissionService service,
            ILogger<StudentAdmissionsController> logger)
        {
            _service = service;
            _logger = logger;
        }


        // =====================================================
        // GET ALL
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _service.GetAllAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while getting all student admissions.");

                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving admissions."
                });
            }
        }


        // =====================================================
        // GET BY ID
        // =====================================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid admission ID."
                    });
                }

                var result =
                    await _service.GetByIdAsync(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Admission not found."
                    });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while getting admission {AdmissionId}.",
                    id);

                return StatusCode(500, new
                {
                    message = "An error occurred while retrieving the admission."
                });
            }
        }


        // =====================================================
        // CREATE
        // =====================================================

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create(
            [FromForm] CreateStudentAdmissionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var result =
                    await _service.CreateAsync(request);

                return CreatedAtAction(
                    nameof(GetById),
                    new
                    {
                        id = result.AdmissionId
                    },
                    result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    innerMessage = ex.InnerException?.Message
                });
            }
        }



        // =====================================================
        // UPDATE
        // =====================================================

        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(
            int id,
            [FromForm] UpdateStudentAdmissionRequest request)
        {
            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid admission ID."
                    });
                }

                var result =
                    await _service.UpdateAsync(
                        id,
                        request);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Admission not found."
                    });
                }

                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while updating admission {AdmissionId}.",
                    id);

                return StatusCode(500, new
                {
                    message = "An error occurred while updating the admission."
                });
            }
        }


        // =====================================================
        // DELETE / SOFT DELETE
        // =====================================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid admission ID."
                    });
                }

                var result =
                    await _service.DeleteAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Admission not found."
                    });
                }

                return Ok(new
                {
                    message = "Admission deleted successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while deleting admission {AdmissionId}.",
                    id);

                return StatusCode(500, new
                {
                    message = "An error occurred while deleting the admission."
                });
            }
        }


        //validation//
        [HttpPost("{id:int}/submit")]

        public async Task<IActionResult> SubmitAdmission(int id)
        {
            try
            {
                var result = await _service.SubmitAsync(id);

                return Ok(new
                {
                    success = true,
                    message = "Admission submitted successfully.",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // =====================================================
        // VERIFY
        // =====================================================

        [HttpPost("{id:int}/verify")]
        public async Task<IActionResult> Verify(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid admission ID."
                    });
                }

                var result =
                    await _service.VerifyAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message = "Admission not found or cannot be verified."
                    });
                }

                return Ok(new
                {
                    message = "Admission verified successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    innerMessage = ex.InnerException?.Message
                });
            }
        }


        // =====================================================
        // APPROVE
        // Creates Student + generates RollNo
        // =====================================================

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid admission ID."
                    });
                }

                var result =
                    await _service.ApproveAsync(id);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message =
                            "Admission not found or cannot be approved."
                    });
                }

                return Ok(new
                {
                    message =
                        "Admission approved successfully.",

                    studentId =
                        result.StudentId,

                    rollNo =
                        result.RollNo
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = ex.Message,
                    innerMessage = ex.InnerException?.Message
                });
            }
        }


        // =====================================================
        // REJECT
        // =====================================================

        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new
                    {
                        message = "Invalid admission ID."
                    });
                }

                var result =
                    await _service.RejectAsync(id);

                if (!result)
                {
                    return NotFound(new
                    {
                        message =
                            "Admission not found or cannot be rejected."
                    });
                }

                return Ok(new
                {
                    message =
                        "Admission rejected successfully."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while rejecting admission {AdmissionId}.",
                    id);

                return StatusCode(500, new
                {
                    message = "An error occurred while rejecting the admission."
                });
            }
        }


        // =====================================================
        // BLOOD GROUPS
        // =====================================================

        [HttpGet("blood-groups")]
        public IActionResult GetBloodGroups()
        {
            var bloodGroups = new[]
            {
                "A+",
                "A-",
                "B+",
                "B-",
                "AB+",
                "AB-",
                "O+",
                "O-"
            };

            return Ok(new
            {
                statusCode = 200,
                message = "Blood groups retrieved successfully.",
                data = bloodGroups
            });
        }

        // =====================================================
        // GENERATE ADMISSION NUMBER
        // =====================================================
        [HttpPost("generate-number")]
        public async Task<IActionResult> GenerateAdmissionNumber()
        {
            var admissionNo =
                await _service.GenerateAdmissionNumberAsync();

            return Ok(new
            {
                admissionNumber = admissionNo
            });
        }
    }
}
