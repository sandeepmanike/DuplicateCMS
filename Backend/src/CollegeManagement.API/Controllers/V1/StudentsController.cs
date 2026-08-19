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
            var result = await _service.GetAllAsync();

            return Ok(result);
        }


        // =========================================================
        // GET STUDENT BY ID
        // =========================================================

        [HttpGet("{studentId:int}")]
        public async Task<IActionResult> GetById(
            int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            var result =
                await _service.GetByIdAsync(studentId);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

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
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            try
            {
                var result =
                    await _service.UpdateAsync(
                        studentId,
                        request);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Student not found."
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // =========================================================
        // DELETE STUDENT
        // =========================================================

        [HttpDelete("{studentId:int}")]
        public async Task<IActionResult> Delete(
            int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            var result =
                await _service.DeleteAsync(studentId);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            return Ok(new
            {
                message =
                    "Student deleted successfully."
            });
        }


        // =========================================================
        // GET STUDENT PROFILE
        // =========================================================

        [HttpGet("{studentId:int}/profile")]
        public async Task<IActionResult> GetProfile(
            int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            var result =
                await _service.GetProfileAsync(studentId);

            if (result == null)
            {
                return NotFound(new
                {
                    message =
                        "Student profile not found."
                });
            }

            return Ok(result);
        }


        // =========================================================
        // UPDATE STUDENT PROFILE
        // =========================================================

        [HttpPut("{studentId:int}/profile")]
        public async Task<IActionResult> UpdateProfile(
            int studentId,
            [FromBody] UpdateStudentProfileRequest request)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result =
                await _service.UpdateProfileAsync(
                    studentId,
                    request);

            if (result == null)
            {
                return NotFound(new
                {
                    message =
                        "Student profile not found."
                });
            }

            return Ok(result);
        }


        // =========================================================
        // CHANGE SECTION
        // =========================================================

        [HttpPatch("{studentId:int}/section")]
        public async Task<IActionResult> ChangeSection(
            int studentId,
            [FromBody] ChangeSectionRequest request)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result =
                await _service.ChangeSectionAsync(
                    studentId,
                    request);

            if (!result)
            {
                return NotFound(new
                {
                    message =
                        "Student or Section not found."
                });
            }

            return Ok(new
            {
                success = true,
                message =
                    "Student section updated successfully."
            });
        }


        // =========================================================
        // CHANGE GROUP
        // =========================================================

        [HttpPatch("{studentId:int}/group")]
        public async Task<IActionResult> ChangeGroup(
            int studentId,
            [FromBody] ChangeGroupRequest request)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result =
                await _service.ChangeGroupAsync(
                    studentId,
                    request);

            if (!result)
            {
                return NotFound(new
                {
                    message =
                        "Student or Group not found."
                });
            }

            return Ok(new
            {
                success = true,
                message =
                    "Student group updated successfully."
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
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result =
                await _service.TransferAsync(
                    studentId,
                    request);

            if (!result)
            {
                return NotFound(new
                {
                    message =
                        "Student or academic reference not found."
                });
            }

            return Ok(new
            {
                success = true,
                message =
                    "Student transferred successfully."
            });
        }


        // =========================================================
        // SUSPEND STUDENT
        // =========================================================

        [HttpPatch("{studentId:int}/suspend")]
        public async Task<IActionResult> Suspend(
            int studentId,
            [FromBody] SuspendStudentRequest request)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(ModelState);
            }

            var result =
                await _service.SuspendAsync(
                    studentId,
                    request);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            return Ok(new
            {
                success = true,
                message =
                    "Student suspended successfully."
            });
        }


        // =========================================================
        // ACTIVATE STUDENT
        // =========================================================

        [HttpPatch("{studentId:int}/activate")]
        public async Task<IActionResult> Activate(
            int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            var result =
                await _service.ActivateAsync(studentId);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            return Ok(new
            {
                success = true,
                message =
                    "Student activated successfully."
            });
        }


        // =========================================================
        // RESET PASSWORD
        // =========================================================

        [HttpPost("{studentId:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(
            int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            var result =
                await _service.ResetPasswordAsync(
                    studentId);

            if (!result)
            {
                return NotFound(new
                {
                    message = "Student not found."
                });
            }

            return Ok(new
            {
                success = true,
                message =
                    "Student password reset successfully."
            });
        }


        // =========================================================
        // STUDENT DASHBOARD
        // =========================================================

        [HttpGet("{studentId:int}/dashboard")]
        public async Task<IActionResult> GetDashboard(
            int studentId)
        {
            if (studentId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid StudentId is required."
                });
            }

            var result =
                await _service.GetDashboardAsync(
                    studentId);

            if (result == null)
            {
                return NotFound(new
                {
                    message =
                        "Student dashboard not found."
                });
            }

            return Ok(result);
        }


        // =========================================================
        // SEARCH STUDENTS
        // =========================================================

        [HttpGet("search")]
        public async Task<IActionResult> Search(
            [FromQuery] string? search = null,
            [FromQuery] int? groupId = null,
            [FromQuery] int? sectionId = null,
            [FromQuery] int? academicYearId = null,
            [FromQuery] bool? isActive = null)
        {
            var result =
                await _service.SearchAsync(
                    search,
                    groupId,
                    sectionId,
                    academicYearId,
                    isActive);

            return Ok(result);
        }


        // =========================================================
        // GET ACTIVE STUDENTS
        // =========================================================

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var result =
                await _service.GetActiveAsync();

            return Ok(result);
        }


        // =========================================================
        // GET STUDENTS BY GROUP
        // =========================================================

        [HttpGet("group/{groupId:int}")]
        public async Task<IActionResult> GetByGroup(
            int groupId)
        {
            if (groupId <= 0)
            {
                return BadRequest(new
                {
                    message = "Valid GroupId is required."
                });
            }

            var result =
                await _service.GetByGroupAsync(
                    groupId);

            return Ok(result);
        }


        // =========================================================
        // GET STUDENTS BY SECTION
        // =========================================================

        [HttpGet("section/{sectionId:int}")]
        public async Task<IActionResult> GetBySection(
            int sectionId)
        {
            if (sectionId <= 0)
            {
                return BadRequest(new
                {
                    message =
                        "Valid SectionId is required."
                });
            }

            var result =
                await _service.GetBySectionAsync(
                    sectionId);

            return Ok(result);
        }


        // =========================================================
        // CHECK EMAIL
        // =========================================================

        [HttpGet("check-email")]
        public async Task<IActionResult> CheckEmail(
            [FromQuery] string email,
            [FromQuery] int? excludeStudentId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new
                {
                    message = "Email is required."
                });
            }

            var isValid =
                System.Text.RegularExpressions.Regex.IsMatch(
                    email,
                    @"^[A-Za-z0-9._%+-]+@gmail\.com$");

            if (!isValid)
            {
                return BadRequest(new
                {
                    message =
                        "Email must be a valid @gmail.com address."
                });
            }

            var exists =
                await _service.EmailExistsAsync(
                    email,
                    excludeStudentId);

            return Ok(new
            {
                email,
                exists,
                isAvailable = !exists
            });
        }


        // =========================================================
        // CHECK MOBILE
        // =========================================================

        [HttpGet("check-mobile")]
        public async Task<IActionResult> CheckMobile(
            [FromQuery] string mobileNumber,
            [FromQuery] int? excludeStudentId = null)
        {
            if (string.IsNullOrWhiteSpace(mobileNumber))
            {
                return BadRequest(new
                {
                    message =
                        "Mobile number is required."
                });
            }

            var isValid =
                System.Text.RegularExpressions.Regex.IsMatch(
                    mobileNumber,
                    @"^[6-9][0-9]{9}$");

            if (!isValid)
            {
                return BadRequest(new
                {
                    message =
                        "Mobile number must be exactly 10 digits and start with 6-9."
                });
            }

            var exists =
                await _service.MobileExistsAsync(
                    mobileNumber,
                    excludeStudentId);

            return Ok(new
            {
                mobileNumber,
                exists,
                isAvailable = !exists
            });
        }
    }
}