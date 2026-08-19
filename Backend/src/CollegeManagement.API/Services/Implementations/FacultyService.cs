using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.DTOs.Faculty;
using CollegeManagement.API.DTOs.Faculty.Request;
using CollegeManagement.API.DTOs.Faculty.Response;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Models.Faculty;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace CollegeManagement.API.Services.Implementations
{
    public class FacultyService : IFacultyService
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IFacultySubjectAllocationRepository _allocationRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ITimetableRepository _timetableRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;

        private const decimal HoursPerClassPeriod = 1.0m;
        private const long MaxPhotoFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] AllowedPhotoExtensions = { ".jpg", ".jpeg", ".png" };

        public FacultyService(
            IFacultyRepository facultyRepository,
            IFacultySubjectAllocationRepository allocationRepository,
            IDepartmentRepository departmentRepository,
            ITimetableRepository timetableRepository,
            IMapper mapper,
            IWebHostEnvironment environment)
        {
            _facultyRepository = facultyRepository;
            _allocationRepository = allocationRepository;
            _departmentRepository = departmentRepository;
            _timetableRepository = timetableRepository;
            _mapper = mapper;
            _environment = environment;
        }

        public async Task<PagedResult<FacultyResponseDto>> GetPagedFacultiesAsync(FacultyQueryParams queryParams)
        {
            var (faculties, totalCount) = await _facultyRepository.GetPagedFacultiesAsync(queryParams);
            var dtos = _mapper.Map<List<FacultyResponseDto>>(faculties);

            return new PagedResult<FacultyResponseDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                PageNumber = queryParams.PageNumber,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<FacultyResponseDto?> GetFacultyByIdAsync(int id)
        {
            var faculty = await _facultyRepository.GetByIdAsync(id);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {id} not found.");

            return _mapper.Map<FacultyResponseDto>(faculty);
        }

        public async Task<FacultyResponseDto?> GetFacultyByEmployeeIdAsync(string employeeId)
        {
            var faculty = await _facultyRepository.GetByEmployeeIdAsync(employeeId);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with Employee ID {employeeId} not found.");

            return _mapper.Map<FacultyResponseDto>(faculty);
        }

        public async Task<FacultyResponseDto> CreateFacultyAsync(CreateFacultyDto dto)
        {
            // Uniqueness Validations
            if (!await _facultyRepository.IsEmployeeIdUniqueAsync(dto.EmployeeId))
                throw new ConflictException($"Employee ID '{dto.EmployeeId}' is already registered.");

            if (!await _facultyRepository.IsEmailUniqueAsync(dto.Email))
                throw new ConflictException($"Email address '{dto.Email}' is already registered.");

            if (!await _facultyRepository.IsMobileUniqueAsync(dto.Mobile))
                throw new ConflictException($"Mobile number '{dto.Mobile}' is already registered.");

            if (!await _facultyRepository.IsAadhaarUniqueAsync(dto.Aadhaar))
                throw new ConflictException($"Aadhaar number '{dto.Aadhaar}' is already registered.");

            // Department resolution
            int? resolvedDepartmentId = dto.DepartmentId;
            if (!resolvedDepartmentId.HasValue || resolvedDepartmentId.Value <= 0)
            {
                if (!string.IsNullOrWhiteSpace(dto.Department))
                {
                    var depts = await _departmentRepository.GetActiveDepartmentsAsync();
                    var dept = depts?.FirstOrDefault(d =>
                        string.Equals(d.DepartmentName, dto.Department.Trim(), StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(d.DepartmentCode, dto.Department.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (dept != null)
                        resolvedDepartmentId = dept.DepartmentId;
                }
            }

            var faculty = _mapper.Map<Faculty>(dto);
            faculty.DepartmentId = resolvedDepartmentId;

            var createdFaculty = await _facultyRepository.AddAsync(faculty);
            return _mapper.Map<FacultyResponseDto>(createdFaculty);
        }

        public async Task<FacultyResponseDto> UpdateFacultyAsync(int id, UpdateFacultyDto dto)
        {
            var existingFaculty = await _facultyRepository.GetByIdAsync(id);
            if (existingFaculty == null)
                throw new NotFoundException($"Faculty record with ID {id} not found.");

            // Uniqueness Validations (Excluding current ID)
            if (!await _facultyRepository.IsEmailUniqueAsync(dto.Email, id))
                throw new ConflictException($"Email address '{dto.Email}' is already registered to another faculty.");

            if (!await _facultyRepository.IsMobileUniqueAsync(dto.Mobile, id))
                throw new ConflictException($"Mobile number '{dto.Mobile}' is already registered to another faculty.");

            if (!await _facultyRepository.IsAadhaarUniqueAsync(dto.Aadhaar, id))
                throw new ConflictException($"Aadhaar number '{dto.Aadhaar}' is already registered to another faculty.");

            // Department resolution
            int? resolvedDepartmentId = dto.DepartmentId;
            if (!resolvedDepartmentId.HasValue || resolvedDepartmentId.Value <= 0)
            {
                if (!string.IsNullOrWhiteSpace(dto.Department))
                {
                    var depts = await _departmentRepository.GetActiveDepartmentsAsync();
                    var dept = depts?.FirstOrDefault(d =>
                        string.Equals(d.DepartmentName, dto.Department.Trim(), StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(d.DepartmentCode, dto.Department.Trim(), StringComparison.OrdinalIgnoreCase));
                    if (dept != null)
                        resolvedDepartmentId = dept.DepartmentId;
                }
            }

            _mapper.Map(dto, existingFaculty);
            existingFaculty.DepartmentId = resolvedDepartmentId;
            existingFaculty.UpdatedAt = DateTime.UtcNow;

            await _facultyRepository.UpdateAsync(existingFaculty);
            return _mapper.Map<FacultyResponseDto>(existingFaculty);
        }

        public async Task<bool> DeleteFacultyAsync(int id)
        {
            var faculty = await _facultyRepository.GetByIdAsync(id);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {id} not found.");

            await _facultyRepository.SoftDeleteAsync(faculty);
            return true;
        }

        public async Task<FacultyResponseDto> UploadPhotoAsync(UploadFacultyPhotoDto dto)
        {
            var faculty = await _facultyRepository.GetByIdAsync(dto.FacultyId);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {dto.FacultyId} not found.");

            if (dto.Photo == null || dto.Photo.Length == 0)
                throw new ValidationException("Please provide a valid non-empty photo file.");

            if (dto.Photo.Length > MaxPhotoFileSizeBytes)
                throw new ValidationException("Photo file size cannot exceed 5 MB.");

            var fileExtension = Path.GetExtension(dto.Photo.FileName).ToLowerInvariant();
            if (!AllowedPhotoExtensions.Contains(fileExtension, StringComparer.OrdinalIgnoreCase))
                throw new ValidationException("Invalid photo file format. Only .jpg, .jpeg, and .png are allowed.");

            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsDirectory = Path.Combine(webRootPath, "uploads", "faculties");

            if (!Directory.Exists(uploadsDirectory))
                Directory.CreateDirectory(uploadsDirectory);

            var fileName = $"faculty_{faculty.Id}_{DateTime.UtcNow.Ticks}{fileExtension}";
            var fullPath = Path.Combine(uploadsDirectory, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await dto.Photo.CopyToAsync(stream);
            }

            var relativePhotoPath = $"/uploads/faculties/{fileName}";
            await _facultyRepository.UpdatePhotoPathAsync(faculty.Id, relativePhotoPath);

            faculty.PhotoPath = relativePhotoPath;
            return _mapper.Map<FacultyResponseDto>(faculty);
        }

        public async Task<(string PhysicalPath, string ContentType)> GetPhotoAsync(int id)
        {
            var photoPath = await _facultyRepository.GetPhotoPathAsync(id);
            if (string.IsNullOrWhiteSpace(photoPath))
            {
                throw new NotFoundException($"Photo for faculty ID {id} does not exist.");
            }

            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var relativePath = photoPath.TrimStart('/', '\\');
            var physicalPath = Path.Combine(webRootPath, relativePath);

            if (!File.Exists(physicalPath))
            {
                throw new NotFoundException($"Photo file for faculty ID {id} not found on disk.");
            }

            var extension = Path.GetExtension(physicalPath).ToLowerInvariant();
            var contentType = extension switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };

            return (physicalPath, contentType);
        }

        public async Task<FacultySubjectAllocationResponseDto> AssignSubjectAsync(AssignSubjectDto dto)
        {
            // Verify Faculty Exists
            var faculty = await _facultyRepository.GetByIdAsync(dto.FacultyId);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {dto.FacultyId} not found.");

            // Verify Faculty is Active
            if (string.Equals(faculty.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Cannot assign subject allocation to an inactive faculty member.");

            // Verify Faculty is Teaching type
            if (string.Equals(faculty.FacultyType, "Non-Teaching", StringComparison.OrdinalIgnoreCase))
                throw new ValidationException("Non-Teaching faculty cannot be assigned subject allocations.");

            // Resolve Subject ID (supports direct SubjectId or string name/code fallback)
            var targetSubjectName = !string.IsNullOrWhiteSpace(dto.Subject) ? dto.Subject : (!string.IsNullOrWhiteSpace(dto.SubjectName) ? dto.SubjectName : dto.SubjectCode);
            var resolvedSubjectId = await _allocationRepository.ResolveSubjectIdAsync(
                dto.SubjectId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, targetSubjectName ?? string.Empty);

            if (!resolvedSubjectId.HasValue || resolvedSubjectId.Value <= 0)
                throw new ValidationException("Please provide a valid Subject ID or Subject Name.");

            int finalSubjectId = resolvedSubjectId.Value;

            // Prevent Duplicate Subject Allocation
            if (await _allocationRepository.ExistsAllocationAsync(dto.FacultyId, finalSubjectId))
                throw new ConflictException($"Subject ID {finalSubjectId} is already allocated to Faculty ID {dto.FacultyId}.");

            var allocation = new FacultySubjectAllocation
            {
                FacultyId = dto.FacultyId,
                SubjectId = finalSubjectId
            };

            var createdAllocation = await _allocationRepository.AddAsync(allocation);
            var fullAllocation = await _allocationRepository.GetByIdAsync(createdAllocation.Id) ?? createdAllocation;
            var resultDto = _mapper.Map<FacultySubjectAllocationResponseDto>(fullAllocation);
            resultDto.FacultyName = $"{faculty.FirstName} {faculty.LastName}".Trim();
            return resultDto;
        }

        public async Task<FacultySubjectAllocationResponseDto> UpdateSubjectAllocationAsync(int id, UpdateSubjectAllocationDto dto)
        {
            var existingAllocation = await _allocationRepository.GetByIdAsync(id);
            if (existingAllocation == null)
                throw new NotFoundException($"Subject Allocation record with ID {id} not found.");

            // Verify Faculty is Teaching type
            var faculty = await _facultyRepository.GetByIdAsync(existingAllocation.FacultyId);
            if (faculty != null)
            {
                if (string.Equals(faculty.Status, "Inactive", StringComparison.OrdinalIgnoreCase))
                    throw new ValidationException("Cannot update subject allocation for an inactive faculty member.");

                if (string.Equals(faculty.FacultyType, "Non-Teaching", StringComparison.OrdinalIgnoreCase))
                    throw new ValidationException("Non-Teaching faculty cannot be assigned subject allocations.");
            }

            var targetSubjectName = !string.IsNullOrWhiteSpace(dto.Subject) ? dto.Subject : (!string.IsNullOrWhiteSpace(dto.SubjectName) ? dto.SubjectName : dto.SubjectCode);
            var resolvedSubjectId = await _allocationRepository.ResolveSubjectIdAsync(
                dto.SubjectId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, targetSubjectName ?? string.Empty);

            if (!resolvedSubjectId.HasValue || resolvedSubjectId.Value <= 0)
                throw new ValidationException("Please provide a valid Subject ID or Subject Name.");

            int finalSubjectId = resolvedSubjectId.Value;

            // Check Duplicate (excluding current allocation id)
            if (await _allocationRepository.ExistsAllocationAsync(existingAllocation.FacultyId, finalSubjectId, id))
                throw new ConflictException($"Subject ID {finalSubjectId} is already allocated to Faculty ID {existingAllocation.FacultyId}.");

            existingAllocation.SubjectId = finalSubjectId;

            await _allocationRepository.UpdateAsync(existingAllocation);

            var updatedAllocation = await _allocationRepository.GetByIdAsync(id);
            return _mapper.Map<FacultySubjectAllocationResponseDto>(updatedAllocation);
        }

        public async Task<bool> DeleteSubjectAllocationAsync(int id)
        {
            var existingAllocation = await _allocationRepository.GetByIdAsync(id);
            if (existingAllocation == null)
                throw new NotFoundException($"Subject Allocation record with ID {id} not found.");

            await _allocationRepository.DeleteAsync(existingAllocation);
            return true;
        }

        private async Task EnrichAllocationSectionsAsync(int facultyId, List<FacultySubjectAllocationResponseDto> allocationDtos)
        {
            if (allocationDtos == null || !allocationDtos.Any()) return;

            var timetableSlots = await _timetableRepository.GetByFacultyIdAsync(facultyId, null);
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

        public async Task<List<FacultySubjectAllocationResponseDto>> GetFacultySubjectAllocationsAsync(int facultyId)
        {
            var faculty = await _facultyRepository.GetByIdAsync(facultyId);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {facultyId} not found.");

            var allocations = await _allocationRepository.GetByFacultyIdAsync(facultyId);
            var dtos = _mapper.Map<List<FacultySubjectAllocationResponseDto>>(allocations);
            await EnrichAllocationSectionsAsync(facultyId, dtos);
            return dtos;
        }

        public async Task<FacultyWorkloadResponseDto?> GetFacultyWorkloadAsync(int facultyId)
        {
            var faculty = await _facultyRepository.GetByIdAsync(facultyId);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {facultyId} not found.");

            var allocations = await _allocationRepository.GetByFacultyIdAsync(facultyId);
            var allocationDtos = _mapper.Map<List<FacultySubjectAllocationResponseDto>>(allocations);
            await EnrichAllocationSectionsAsync(facultyId, allocationDtos);

            var timetableSlots = await _timetableRepository.GetByFacultyIdAsync(facultyId, null);
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

            return new FacultyWorkloadResponseDto
            {
                FacultyId = faculty.Id,
                FacultyName = $"{faculty.FirstName} {faculty.LastName}".Trim(),
                EmployeeId = faculty.EmployeeId,
                Department = faculty.Department,
                Designation = faculty.Designation,
                TotalAssignedSubjects = totalAssignedSubjects,
                TotalSections = totalSections,
                WeeklyClasses = weeklyClasses,
                TotalWorkloadHours = Math.Round(totalWorkloadHours, 2),
                Allocations = allocationDtos
            };
        }
    }
}
