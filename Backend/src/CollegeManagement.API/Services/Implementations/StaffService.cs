using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.DTOs.Staff;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Models;
using CollegeManagement.API.Models.Staff;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace CollegeManagement.API.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;
        private readonly IStaffSubjectAllocationRepository _allocationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ITimetableRepository _timetableRepository;
        private readonly IDesignationRepository _designationRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        private const decimal HoursPerClassPeriod = 1.0m;
        private const long MaxPhotoFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] AllowedPhotoExtensions = { ".jpg", ".jpeg", ".png" };

        public StaffService(
            IStaffRepository staffRepository,
            IStaffSubjectAllocationRepository allocationRepository,
            IDepartmentRepository departmentRepository,
            ITimetableRepository timetableRepository,
            IDesignationRepository designationRepository,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _staffRepository = staffRepository;
            _allocationRepository = allocationRepository;
            _departmentRepository = departmentRepository;
            _timetableRepository = timetableRepository;
            _designationRepository = designationRepository;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<PagedResult<StaffResponseDto>> GetPagedStaffAsync(StaffQueryParams queryParams)
        {
            var (staffs, totalCount) = await _staffRepository.GetPagedStaffAsync(queryParams);
            var dtos = _mapper.Map<List<StaffResponseDto>>(staffs);

            return new PagedResult<StaffResponseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<IEnumerable<StaffDropdownDto>> GetStaffDropdownAsync(string? staffType = null)
        {
            return await _staffRepository.GetStaffDropdownAsync(staffType);
        }

        public async Task<StaffResponseDto?> GetStaffByIdAsync(int id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null)
                throw new NotFoundException($"Staff record with ID {id} not found.");

            return _mapper.Map<StaffResponseDto>(staff);
        }

        public async Task<StaffResponseDto?> GetStaffByEmployeeIdAsync(string employeeId)
        {
            var staff = await _staffRepository.GetByEmployeeIdAsync(employeeId);
            if (staff == null)
                throw new NotFoundException($"Staff record with Employee ID {employeeId} not found.");

            return _mapper.Map<StaffResponseDto>(staff);
        }

        public async Task<string> GetNextEmployeeIdAsync(string staffType)
        {
            return await _staffRepository.GenerateNextEmployeeIdAsync(staffType);
        }

        public async Task<StaffResponseDto> CreateStaffAsync(CreateStaffDto dto)
        {
            var staffType = string.IsNullOrWhiteSpace(dto.StaffType) ? "Teaching" : dto.StaffType.Trim();

            // Auto generate Employee ID if not supplied or empty
            string employeeId = dto.EmployeeId?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(employeeId))
            {
                employeeId = await _staffRepository.GenerateNextEmployeeIdAsync(staffType);
            }

            // Uniqueness Validations
            if (!await _staffRepository.IsEmployeeIdUniqueAsync(employeeId))
                throw new ConflictException($"Employee ID '{employeeId}' is already registered.");

            if (!await _staffRepository.IsEmailUniqueAsync(dto.Email))
                throw new ConflictException($"Email address '{dto.Email}' is already registered.");

            if (!await _staffRepository.IsMobileUniqueAsync(dto.Mobile))
                throw new ConflictException($"Mobile number '{dto.Mobile}' is already registered.");

            if (!string.IsNullOrWhiteSpace(dto.Aadhaar) && !await _staffRepository.IsAadhaarUniqueAsync(dto.Aadhaar))
                throw new ConflictException($"Aadhaar number '{dto.Aadhaar}' is already registered.");

            // Department resolution & auto creation if custom ("Other")
            int? resolvedDepartmentId = dto.DepartmentId;
            string deptName = dto.Department?.Trim() ?? string.Empty;

            if (resolvedDepartmentId.HasValue && resolvedDepartmentId.Value > 0)
            {
                var depts = await _departmentRepository.GetDepartmentsAsync();
                var found = depts.FirstOrDefault(d => d.DepartmentId == resolvedDepartmentId.Value);
                if (found != null)
                {
                    deptName = found.DepartmentName;
                }
            }
            else if (!string.IsNullOrWhiteSpace(deptName))
            {
                var depts = await _departmentRepository.GetDepartmentsAsync();
                var dept = depts?.FirstOrDefault(d =>
                    string.Equals(d.DepartmentName, deptName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(d.DepartmentCode, deptName, StringComparison.OrdinalIgnoreCase));

                if (dept != null)
                {
                    resolvedDepartmentId = dept.DepartmentId;
                    deptName = dept.DepartmentName;
                }
                else
                {
                    // Create new department dynamically
                    var newDept = new Department
                    {
                        DepartmentName = deptName,
                        DepartmentCode = $"DEP_{deptName.Substring(0, Math.Min(8, deptName.Length)).ToUpper().Replace(" ", "")}",
                        StaffType = staffType,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    var createdDept = await _departmentRepository.AddDepartmentAsync(newDept);
                    resolvedDepartmentId = createdDept.DepartmentId;
                }
            }

            // Designation resolution & auto creation if custom ("Other")
            int? resolvedDesignationId = dto.DesignationId;
            string resolvedDesignationName = dto.Designation?.Trim() ?? string.Empty;

            if (resolvedDesignationId.HasValue && resolvedDesignationId.Value > 0)
            {
                var desig = await _designationRepository.GetByIdAsync(resolvedDesignationId.Value);
                if (desig == null)
                    throw new NotFoundException($"Designation with ID {resolvedDesignationId.Value} not found.");

                if (!desig.IsActive)
                    throw new ValidationException($"Designation '{desig.Name}' is inactive and cannot be assigned.");

                resolvedDesignationName = desig.Name;
            }
            else if (!string.IsNullOrWhiteSpace(resolvedDesignationName))
            {
                var desig = await _designationRepository.GetByNameAsync(resolvedDesignationName);
                if (desig != null)
                {
                    resolvedDesignationId = desig.Id;
                    resolvedDesignationName = desig.Name;
                }
                else
                {
                    // Create new designation dynamically
                    var newDesig = new CollegeManagement.API.Models.Faculty.Designation
                    {
                        Name = resolvedDesignationName,
                        StaffType = staffType,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    var createdDesig = await _designationRepository.AddAsync(newDesig);
                    resolvedDesignationId = createdDesig.Id;
                }
            }

            var staff = _mapper.Map<Staff>(dto);
            staff.EmployeeId = employeeId;
            staff.StaffType = staffType;
            staff.DepartmentId = resolvedDepartmentId;
            staff.DesignationId = resolvedDesignationId;
            staff.Designation = resolvedDesignationName;

            var createdStaff = await _staffRepository.AddAsync(staff);
            createdStaff.Department = deptName;
            return _mapper.Map<StaffResponseDto>(createdStaff);
        }

        public async Task<StaffResponseDto> UpdateStaffAsync(int id, UpdateStaffDto dto)
        {
            var existingStaff = await _staffRepository.GetByIdAsync(id);
            if (existingStaff == null)
                throw new NotFoundException($"Staff record with ID {id} not found.");

            // Uniqueness Validations (Excluding current ID)
            if (!await _staffRepository.IsEmailUniqueAsync(dto.Email, id))
                throw new ConflictException($"Email address '{dto.Email}' is already registered to another staff member.");

            if (!await _staffRepository.IsMobileUniqueAsync(dto.Mobile, id))
                throw new ConflictException($"Mobile number '{dto.Mobile}' is already registered to another staff member.");

            if (!string.IsNullOrWhiteSpace(dto.Aadhaar) && !await _staffRepository.IsAadhaarUniqueAsync(dto.Aadhaar, id))
                throw new ConflictException($"Aadhaar number '{dto.Aadhaar}' is already registered to another staff member.");

            var staffType = string.IsNullOrWhiteSpace(dto.StaffType) ? existingStaff.StaffType : dto.StaffType.Trim();

            // Department resolution
            int? resolvedDepartmentId = dto.DepartmentId;
            string deptName = dto.Department?.Trim() ?? string.Empty;

            if (resolvedDepartmentId.HasValue && resolvedDepartmentId.Value > 0)
            {
                var depts = await _departmentRepository.GetDepartmentsAsync();
                var found = depts.FirstOrDefault(d => d.DepartmentId == resolvedDepartmentId.Value);
                if (found != null)
                {
                    deptName = found.DepartmentName;
                }
            }
            else if (!string.IsNullOrWhiteSpace(deptName))
            {
                var depts = await _departmentRepository.GetDepartmentsAsync();
                var dept = depts?.FirstOrDefault(d =>
                    string.Equals(d.DepartmentName, deptName, StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(d.DepartmentCode, deptName, StringComparison.OrdinalIgnoreCase));

                if (dept != null)
                {
                    resolvedDepartmentId = dept.DepartmentId;
                    deptName = dept.DepartmentName;
                }
                else
                {
                    var newDept = new Department
                    {
                        DepartmentName = deptName,
                        DepartmentCode = $"DEP_{deptName.Substring(0, Math.Min(8, deptName.Length)).ToUpper().Replace(" ", "")}",
                        StaffType = staffType,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    var createdDept = await _departmentRepository.AddDepartmentAsync(newDept);
                    resolvedDepartmentId = createdDept.DepartmentId;
                }
            }
            else
            {
                resolvedDepartmentId = existingStaff.DepartmentId;
                deptName = existingStaff.Department;
            }

            // Designation resolution
            int? resolvedDesignationId = dto.DesignationId;
            string resolvedDesignationName = dto.Designation?.Trim() ?? string.Empty;

            if (resolvedDesignationId.HasValue && resolvedDesignationId.Value > 0)
            {
                var desig = await _designationRepository.GetByIdAsync(resolvedDesignationId.Value);
                if (desig == null)
                    throw new NotFoundException($"Designation with ID {resolvedDesignationId.Value} not found.");

                if (!desig.IsActive && existingStaff.DesignationId != desig.Id)
                    throw new ValidationException($"Designation '{desig.Name}' is inactive and cannot be assigned.");

                resolvedDesignationName = desig.Name;
            }
            else if (!string.IsNullOrWhiteSpace(resolvedDesignationName))
            {
                var desig = await _designationRepository.GetByNameAsync(resolvedDesignationName);
                if (desig != null)
                {
                    resolvedDesignationId = desig.Id;
                    resolvedDesignationName = desig.Name;
                }
                else
                {
                    var newDesig = new CollegeManagement.API.Models.Faculty.Designation
                    {
                        Name = resolvedDesignationName,
                        StaffType = staffType,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    var createdDesig = await _designationRepository.AddAsync(newDesig);
                    resolvedDesignationId = createdDesig.Id;
                }
            }
            else
            {
                resolvedDesignationId = existingStaff.DesignationId;
                resolvedDesignationName = existingStaff.Designation;
            }

            _mapper.Map(dto, existingStaff);
            existingStaff.StaffType = staffType;
            existingStaff.DepartmentId = resolvedDepartmentId;
            existingStaff.Department = deptName;
            existingStaff.DesignationId = resolvedDesignationId;
            existingStaff.Designation = resolvedDesignationName;
            existingStaff.UpdatedAt = DateTime.UtcNow;

            await _staffRepository.UpdateAsync(existingStaff);
            return _mapper.Map<StaffResponseDto>(existingStaff);
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null)
                throw new NotFoundException($"Staff record with ID {id} not found.");

            await _staffRepository.SoftDeleteAsync(staff);
            return true;
        }

        public async Task<StaffResponseDto> UploadPhotoAsync(UploadStaffPhotoDto dto)
        {
            var staff = await _staffRepository.GetByIdAsync(dto.StaffId);
            if (staff == null)
                throw new NotFoundException($"Staff record with ID {dto.StaffId} not found.");

            if (dto.Photo == null || dto.Photo.Length == 0)
                throw new ValidationException("Please select a valid image file to upload.");

            if (dto.Photo.Length > MaxPhotoFileSizeBytes)
                throw new ValidationException("Photo size exceeds the maximum allowed limit of 5 MB.");

            var ext = Path.GetExtension(dto.Photo.FileName).ToLowerInvariant();
            if (!AllowedPhotoExtensions.Contains(ext))
                throw new ValidationException("Invalid photo file format. Only JPG, JPEG, and PNG images are allowed.");

            var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "staff");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"staff_{staff.Id}_{DateTime.UtcNow.Ticks}{ext}";
            var physicalPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await dto.Photo.CopyToAsync(stream);
            }

            if (!string.IsNullOrWhiteSpace(staff.PhotoPath))
            {
                var oldFullPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), staff.PhotoPath.TrimStart('/', '\\'));
                if (File.Exists(oldFullPath))
                {
                    try { File.Delete(oldFullPath); } catch { /* Ignore delete exceptions */ }
                }
            }

            var relativePath = $"/uploads/staff/{uniqueFileName}";
            await _staffRepository.UpdatePhotoPathAsync(staff.Id, relativePath);

            staff.PhotoPath = relativePath;
            return _mapper.Map<StaffResponseDto>(staff);
        }

        public async Task<(string PhysicalPath, string ContentType)> GetPhotoAsync(int id)
        {
            var staff = await _staffRepository.GetByIdAsync(id);
            if (staff == null)
                throw new NotFoundException($"Staff record with ID {id} not found.");

            if (string.IsNullOrWhiteSpace(staff.PhotoPath))
                throw new NotFoundException("No photo available for this staff member.");

            var fullPath = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), staff.PhotoPath.TrimStart('/', '\\'));
            if (!File.Exists(fullPath))
                throw new NotFoundException("The photo file could not be found on the server.");

            var ext = Path.GetExtension(fullPath).ToLowerInvariant();
            var contentType = ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return (fullPath, contentType);
        }

        public async Task<StaffSubjectAllocationResponseDto> AssignSubjectAsync(AssignStaffSubjectDto dto)
        {
            var staff = await _staffRepository.GetByIdAsync(dto.StaffId);
            if (staff == null)
                throw new NotFoundException($"Staff record with ID {dto.StaffId} not found.");

            if (string.Equals(staff.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Cannot assign subjects to an inactive staff member.");

            if (string.Equals(staff.StaffType, "Non-Teaching", StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Non-Teaching staff cannot be assigned subject allocations.");

            var targetSubjectName = !string.IsNullOrWhiteSpace(dto.Subject) ? dto.Subject : (!string.IsNullOrWhiteSpace(dto.SubjectName) ? dto.SubjectName : dto.SubjectCode);
            var resolvedSubjectId = await _allocationRepository.ResolveSubjectIdAsync(
                dto.SubjectId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, targetSubjectName ?? string.Empty);

            if (!resolvedSubjectId.HasValue || resolvedSubjectId.Value <= 0)
                throw new ValidationException("Please provide a valid Subject ID or Subject Name.");

            int finalSubjectId = resolvedSubjectId.Value;

            if (await _allocationRepository.ExistsAllocationAsync(dto.StaffId, finalSubjectId))
                throw new ConflictException($"Subject ID {finalSubjectId} is already allocated to Staff ID {dto.StaffId}.");

            var allocation = new StaffSubjectAllocation
            {
                StaffId = dto.StaffId,
                SubjectId = finalSubjectId
            };

            var createdAllocation = await _allocationRepository.AddAsync(allocation);
            var fullAllocation = await _allocationRepository.GetByIdAsync(createdAllocation.Id) ?? createdAllocation;
            var resultDto = _mapper.Map<StaffSubjectAllocationResponseDto>(fullAllocation);
            resultDto.StaffName = $"{staff.FirstName} {staff.LastName}".Trim();
            return resultDto;
        }

        public async Task<StaffSubjectAllocationResponseDto> UpdateSubjectAllocationAsync(int id, UpdateStaffSubjectAllocationDto dto)
        {
            var existingAllocation = await _allocationRepository.GetByIdAsync(id);
            if (existingAllocation == null)
                throw new NotFoundException($"Subject Allocation record with ID {id} not found.");

            var staff = await _staffRepository.GetByIdAsync(existingAllocation.StaffId);
            if (staff != null)
            {
                if (string.Equals(staff.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
                    throw new ValidationException("Cannot update subject allocation for an inactive staff member.");

                if (string.Equals(staff.StaffType, "Non-Teaching", StringComparison.OrdinalIgnoreCase))
                    throw new ValidationException("Non-Teaching staff cannot be assigned subject allocations.");
            }

            var targetSubjectName = !string.IsNullOrWhiteSpace(dto.Subject) ? dto.Subject : (!string.IsNullOrWhiteSpace(dto.SubjectName) ? dto.SubjectName : dto.SubjectCode);
            var resolvedSubjectId = await _allocationRepository.ResolveSubjectIdAsync(
                dto.SubjectId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, targetSubjectName ?? string.Empty);

            if (!resolvedSubjectId.HasValue || resolvedSubjectId.Value <= 0)
                throw new ValidationException("Please provide a valid Subject ID or Subject Name.");

            int finalSubjectId = resolvedSubjectId.Value;

            if (await _allocationRepository.ExistsAllocationAsync(existingAllocation.StaffId, finalSubjectId, id))
                throw new ConflictException($"Subject ID {finalSubjectId} is already allocated to Staff ID {existingAllocation.StaffId}.");

            existingAllocation.SubjectId = finalSubjectId;

            await _allocationRepository.UpdateAsync(existingAllocation);

            var updatedAllocation = await _allocationRepository.GetByIdAsync(id);
            return _mapper.Map<StaffSubjectAllocationResponseDto>(updatedAllocation);
        }

        public async Task<bool> DeleteSubjectAllocationAsync(int id)
        {
            var existingAllocation = await _allocationRepository.GetByIdAsync(id);
            if (existingAllocation == null)
                throw new NotFoundException($"Subject Allocation record with ID {id} not found.");

            await _allocationRepository.DeleteAsync(existingAllocation);
            return true;
        }

        private async Task EnrichAllocationSectionsAsync(int staffId, List<StaffSubjectAllocationResponseDto> allocationDtos)
        {
            if (allocationDtos == null || !allocationDtos.Any()) return;

            var timetableSlots = await _timetableRepository.GetByFacultyIdAsync(staffId, null);
            if (timetableSlots == null || !timetableSlots.Any()) return;

            var subjectSectionsMap = timetableSlots
                .Where(t => !string.IsNullOrWhiteSpace(t.SectionName))
                .GroupBy(t => t.SubjectId)
                .ToDictionary(
                    g => g.Key,
                    g => string.Join(", ", g.Select(x => x.SectionName).Distinct())
                );

            foreach (var dto in allocationDtos)
            {
                if (subjectSectionsMap.TryGetValue(dto.SubjectId, out var secName) && !string.IsNullOrWhiteSpace(secName))
                {
                    dto.Section = secName;
                    dto.SectionName = secName;
                }
            }
        }

        public async Task<List<StaffSubjectAllocationResponseDto>> GetStaffSubjectAllocationsAsync(int staffId)
        {
            var staff = await _staffRepository.GetByIdAsync(staffId);
            if (staff == null)
                throw new NotFoundException($"Staff record with ID {staffId} not found.");

            var allocations = await _allocationRepository.GetByStaffIdAsync(staffId);
            var dtos = _mapper.Map<List<StaffSubjectAllocationResponseDto>>(allocations);
            await EnrichAllocationSectionsAsync(staffId, dtos);
            return dtos;
        }

        public async Task<StaffWorkloadResponseDto?> GetStaffWorkloadAsync(int staffId)
        {
            var staff = await _staffRepository.GetByIdAsync(staffId);
            if (staff == null)
                throw new NotFoundException($"Staff record with ID {staffId} not found.");

            var allocations = await _allocationRepository.GetByStaffIdAsync(staffId);
            var allocationDtos = _mapper.Map<List<StaffSubjectAllocationResponseDto>>(allocations);
            await EnrichAllocationSectionsAsync(staffId, allocationDtos);

            var timetableSlots = await _timetableRepository.GetByFacultyIdAsync(staffId, null);
            var publishedSlots = timetableSlots != null ? timetableSlots.Where(t => t.IsPublished).ToList() : new List<CollegeManagement.API.DTOs.Timetable.TimetableResponseDto>();

            int totalAssignedSubjects = allocationDtos.Select(a => a.SubjectId).Distinct().Count();
            if (totalAssignedSubjects == 0 && publishedSlots.Count > 0)
            {
                totalAssignedSubjects = publishedSlots.Select(t => t.SubjectId).Distinct().Count();
            }

            int totalSections = publishedSlots.Select(t => t.SectionId).Distinct().Count();
            int weeklyClasses = publishedSlots.Count;

            decimal totalWorkloadHours = 0;
            foreach (var slot in publishedSlots)
            {
                if (slot.EndTime > slot.StartTime)
                {
                    totalWorkloadHours += (decimal)(slot.EndTime - slot.StartTime).TotalHours;
                }
                else
                {
                    totalWorkloadHours += HoursPerClassPeriod;
                }
            }

            return new StaffWorkloadResponseDto
            {
                StaffId = staff.Id,
                StaffName = $"{staff.FirstName} {staff.LastName}".Trim(),
                EmployeeId = staff.EmployeeId,
                Department = staff.Department,
                Designation = staff.Designation,
                TotalAssignedSubjects = totalAssignedSubjects,
                TotalSections = totalSections,
                WeeklyClasses = weeklyClasses,
                TotalWorkloadHours = totalWorkloadHours,
                Allocations = allocationDtos
            };
        }
    }
}
