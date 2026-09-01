using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Asp.Versioning;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Staff;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CollegeManagement.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/staff")]
    [EnableCors("AllowFrontend")]
    [Produces("application/json")]
    public class StaffController : ControllerBase
    {
        private readonly IStaffService _staffService;
        private readonly AppDbContext _db;

        public StaffController(IStaffService staffService, AppDbContext db)
        {
            _staffService = staffService;
            _db = db;
        }

        /// <summary>
        /// 1. GET /api/v1/staff or /api/v1/faculty
        /// Get paged, searched, filtered (by StaffType, Department, Designation, Status) list of staff.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaffById(int id)
        {
            var result = await _staffService.GetStaffByIdAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// 4.1. GET /api/v1/staff/{id}/print-details
        /// Get complete individual staff profile formatted for printable ID card or detail profile.
        /// </summary>
        [HttpGet("{id:int}/print-details")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStaffPrintDetails(int id, CancellationToken ct = default)
        {
            var staff = await _db.Staffs
                .AsNoTracking()
                .Include(s => s.DepartmentRef)
                .Include(s => s.DesignationRef)
                .Include(s => s.BoardRef)
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);

            if (staff == null)
            {
                return NotFound(new { message = $"Staff with ID {id} not found." });
            }

            var fullName = $"{staff.FirstName} {staff.LastName}".Trim();
            var result = new
            {
                staff.Id,
                staff.EmployeeId,
                staff.FirstName,
                staff.LastName,
                FullName = fullName,
                staff.Gender,
                DateOfBirth = staff.DateOfBirth.ToString("yyyy-MM-dd"),
                staff.BloodGroup,
                staff.Mobile,
                staff.Email,
                AadhaarNumber = staff.Aadhaar,
                staff.Qualification,
                BoardName = staff.BoardRef?.BoardName ?? staff.BoardName ?? "Board of Intermediate Education",
                Department = staff.DepartmentRef?.DepartmentName ?? staff.Department ?? "General",
                Designation = staff.DesignationRef?.DesignationName ?? staff.Designation ?? "Lecturer",
                JoiningDate = staff.JoiningDate.ToString("yyyy-MM-dd"),
                ExperienceYears = staff.Experience,
                Status = staff.Status ?? "Active",
                StaffType = staff.StaffType ?? "Teaching",
                PhotoUrl = !string.IsNullOrWhiteSpace(staff.PhotoPath) ? $"/api/v1/staff/photo/{staff.Id}" : null
            };

            return Ok(result);
        }

        /// <summary>
        /// 5. POST /api/v1/staff
        /// Create a new staff member (Teaching or Non-Teaching).
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(StaffResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto dto)
        {
            var result = await _staffService.CreateStaffAsync(dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// 6. PUT /api/v1/staff/{id}
        /// Update an existing staff member.
        /// </summary>
        [HttpPut("{id:int}")]
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStaffPhoto(int id)
        {
            var (physicalPath, contentType) = await _staffService.GetPhotoAsync(id);
            return PhysicalFile(physicalPath, contentType);
        }

        // =========================================================================
        // LOOKUP ENDPOINTS (Blood Groups, Boards, Departments, Designations)
        // =========================================================================

        [HttpGet("lookup/blood-groups")]
        [AllowAnonymous]
        public IActionResult GetBloodGroups()
        {
            var groups = new[] { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };
            return Ok(groups);
        }

        [HttpGet("lookup/boards")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBoards(CancellationToken ct = default)
        {
            var boards = await _db.Boards
                .AsNoTracking()
                .Where(b => b.IsActive)
                .OrderBy(b => b.BoardName)
                .Select(b => new { id = b.BoardId, name = b.BoardName, code = b.BoardCode })
                .ToListAsync(ct);

            return Ok(boards);
        }

        [HttpGet("lookup/departments")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDepartments([FromQuery] string? staffType = null, CancellationToken ct = default)
        {
            var query = _db.Departments.AsNoTracking().Where(d => d.IsActive);
            if (!string.IsNullOrWhiteSpace(staffType))
            {
                query = query.Where(d => d.StaffType == null || d.StaffType == staffType);
            }

            var list = await query
                .OrderBy(d => d.DepartmentName)
                .Select(d => new { id = d.DepartmentId, name = d.DepartmentName, code = d.DepartmentCode })
                .ToListAsync(ct);

            if (!list.Any())
            {
                var fallback = (staffType?.Equals("Non-Teaching", StringComparison.OrdinalIgnoreCase) == true)
                    ? new[] { "Administration", "Accounts & Finance", "Admissions", "Examinations", "Library", "Transport", "Hostel", "Security", "Maintenance" }
                    : new[] { "Mathematics", "Physics", "Chemistry", "Botany", "Zoology", "English", "Telugu", "Hindi", "Sanskrit", "Commerce", "Economics", "Civics", "Computer Science" };

                return Ok(fallback.Select((name, i) => new { id = i + 1, name, code = name.ToUpperInvariant() }));
            }

            return Ok(list);
        }

        [HttpGet("lookup/designations")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDesignations([FromQuery] string? staffType = null, CancellationToken ct = default)
        {
            try
            {
                var query = _db.Designations.AsNoTracking().Where(d => d.IsActive);
                if (!string.IsNullOrWhiteSpace(staffType))
                {
                    query = query.Where(d => d.StaffType == null || d.StaffType == staffType || d.StaffType == "Both");
                }

                var list = await query
                    .OrderBy(d => d.Name)
                    .Select(d => new { id = d.Id, name = d.Name, code = d.Name.ToUpper() })
                    .ToListAsync(ct);

                if (list.Any())
                {
                    return Ok(list);
                }
            }
            catch { }

            var fallback = (staffType?.Equals("Non-Teaching", StringComparison.OrdinalIgnoreCase) == true)
                ? new[] { "Administrative Officer", "Accountant", "Librarian", "Lab Assistant", "Office Assistant", "Clerk", "Receptionist" }
                : new[] { "Junior Lecturer", "Lecturer", "Senior Lecturer", "Subject Teacher", "Head of Department (HOD)", "Academic Coordinator", "Vice Principal", "Principal" };

            return Ok(fallback.Select((name, i) => new { id = i + 1, name, code = name.ToUpperInvariant() }));
        }

        // =========================================================================
        // EXPORT TO EXCEL / CSV & PDF
        // =========================================================================

        [HttpGet("export/excel")]
        [AllowAnonymous]
        public async Task<IActionResult> ExportExcel([FromQuery] StaffQueryParams queryParams, CancellationToken ct = default)
        {
            queryParams.PageSize = 10000;
            queryParams.PageNumber = 1;

            var paged = await _staffService.GetPagedStaffAsync(queryParams);
            var items = paged.Items;

            var sb = new StringBuilder();
            sb.AppendLine("Employee ID,First Name,Last Name,Staff Type,Department,Designation,Board,Gender,Mobile,Email,Joining Date,Status");

            foreach (var s in items)
            {
                var empId = EscapeCsv(s.EmployeeId);
                var fName = EscapeCsv(s.FirstName);
                var lName = EscapeCsv(s.LastName);
                var sType = EscapeCsv(s.StaffType);
                var dept = EscapeCsv(s.Department);
                var desig = EscapeCsv(s.Designation);
                var board = EscapeCsv(s.BoardName ?? s.Board);
                var gender = EscapeCsv(s.Gender);
                var mobile = EscapeCsv(s.Mobile);
                var email = EscapeCsv(s.Email);
                var join = EscapeCsv(s.JoiningDate.ToString("yyyy-MM-dd"));
                var status = EscapeCsv(s.Status);

                sb.AppendLine($"{empId},{fName},{lName},{sType},{dept},{desig},{board},{gender},{mobile},{email},{join},{status}");
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            var filename = $"Staff_Export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";
            return File(bytes, "text/csv; charset=utf-8", filename);
        }

        [HttpGet("export/pdf")]
        [AllowAnonymous]
        public async Task<IActionResult> ExportPdf([FromQuery] StaffQueryParams queryParams, CancellationToken ct = default)
        {
            queryParams.PageSize = 10000;
            queryParams.PageNumber = 1;

            var paged = await _staffService.GetPagedStaffAsync(queryParams);
            var items = paged.Items;
            var totalCount = paged.TotalCount;
            var title = $"Staff Directory ({queryParams.StaffType ?? "All Staff"})";

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Helvetica"));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("PIRNAV COLLEGE - STAFF DIRECTORY")
                            .SemiBold().FontSize(16).FontColor(Colors.Green.Darken2);
                        col.Item().Text($"Generated on {DateTime.UtcNow:dd MMM yyyy, hh:mm tt} | Total Records: {totalCount}")
                            .FontSize(8).FontColor(Colors.Grey.Medium);
                        col.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(75);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn(1.5f);
                            cols.RelativeColumn(2);
                            cols.ConstantColumn(80);
                            cols.ConstantColumn(55);
                        });

                        table.Header(h =>
                        {
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("EMPLOYEE ID").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("NAME").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("DEPARTMENT").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("DESIGNATION").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("BOARD").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("MOBILE").Bold();
                            h.Cell().Background(Colors.Grey.Lighten3).Padding(4).Text("STATUS").Bold();
                        });

                        foreach (var s in items)
                        {
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.EmployeeId ?? "");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text($"{s.FirstName} {s.LastName}".Trim());
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.Department ?? "");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.Designation ?? "");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.BoardName ?? s.Board ?? "—");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.Mobile ?? "");
                            table.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(4).Text(s.Status ?? "Active");
                        }
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Page ");
                        x.CurrentPageNumber();
                        x.Span(" of ");
                        x.TotalPages();
                    });
                });
            });

            using var ms = new MemoryStream();
            doc.GeneratePdf(ms);
            var bytes = ms.ToArray();
            var filename = $"Staff_Directory_{DateTime.UtcNow:yyyyMMdd_HHmmss}.pdf";
            return File(bytes, "application/pdf", filename);
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }
    }
}
