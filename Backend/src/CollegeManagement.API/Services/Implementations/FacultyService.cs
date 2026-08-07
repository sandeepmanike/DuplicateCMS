using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Faculty;
using CollegeManagement.API.DTOs.Faculty.Request;
using CollegeManagement.API.DTOs.Faculty.Response;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Models;
using CollegeManagement.API.Models.Faculty;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services.Implementations
{
    public class FacultyService : IFacultyService
    {
        private readonly IFacultyRepository _facultyRepository;
        private readonly IFacultySubjectAllocationRepository _allocationRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _environment;
        private readonly AppDbContext _context;

        private const int StandardWeeklyClassesPerSubject = 4;
        private const decimal HoursPerClassPeriod = 1.0m;
        private const long MaxPhotoFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        private static readonly string[] AllowedPhotoExtensions = { ".jpg", ".jpeg", ".png" };

        public FacultyService(
            IFacultyRepository facultyRepository,
            IFacultySubjectAllocationRepository allocationRepository,
            IMapper mapper,
            IWebHostEnvironment environment,
            AppDbContext context)
        {
            _facultyRepository = facultyRepository;
            _allocationRepository = allocationRepository;
            _mapper = mapper;
            _environment = environment;
            _context = context;
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
                throw new NotFoundException($"Faculty record with Employee ID '{employeeId}' not found.");

            return _mapper.Map<FacultyResponseDto>(faculty);
        }

        public async Task<FacultyResponseDto> CreateFacultyAsync(CreateFacultyDto dto)
        {
            // Uniqueness Validation
            if (!await _facultyRepository.IsEmployeeIdUniqueAsync(dto.EmployeeId))
                throw new ConflictException($"Employee ID '{dto.EmployeeId}' is already registered.");

            if (!await _facultyRepository.IsEmailUniqueAsync(dto.Email))
                throw new ConflictException($"Email '{dto.Email}' is already registered.");

            if (!await _facultyRepository.IsMobileUniqueAsync(dto.Mobile))
                throw new ConflictException($"Mobile number '{dto.Mobile}' is already registered.");

            if (!await _facultyRepository.IsAadhaarUniqueAsync(dto.Aadhaar))
                throw new ConflictException($"Aadhaar number '{dto.Aadhaar}' is already registered.");

            if (!await _facultyRepository.IsUsernameUniqueAsync(dto.Username))
                throw new ConflictException($"Username '{dto.Username}' is already taken.");

            var faculty = _mapper.Map<Faculty>(dto);

            // Hash Password using BCrypt
            faculty.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            faculty.CreatedAt = DateTime.UtcNow;
            faculty.IsDeleted = false;

            var createdFaculty = await _facultyRepository.AddAsync(faculty);

            return _mapper.Map<FacultyResponseDto>(createdFaculty);
        }

        public async Task<FacultyResponseDto> UpdateFacultyAsync(int id, UpdateFacultyDto dto)
        {
            var existingFaculty = await _facultyRepository.GetByIdAsync(id);
            if (existingFaculty == null)
                throw new NotFoundException($"Faculty record with ID {id} not found.");

            // Uniqueness Validations with ExcludeId
            if (!await _facultyRepository.IsEmailUniqueAsync(dto.Email, id))
                throw new ConflictException($"Email '{dto.Email}' is already registered with another faculty.");

            if (!await _facultyRepository.IsMobileUniqueAsync(dto.Mobile, id))
                throw new ConflictException($"Mobile number '{dto.Mobile}' is already registered with another faculty.");

            if (!await _facultyRepository.IsAadhaarUniqueAsync(dto.Aadhaar, id))
                throw new ConflictException($"Aadhaar number '{dto.Aadhaar}' is already registered with another faculty.");

            _mapper.Map(dto, existingFaculty);
            existingFaculty.UpdatedAt = DateTime.UtcNow;

            await _facultyRepository.UpdateAsync(existingFaculty);

            var updatedFaculty = await _facultyRepository.GetByIdAsync(id);
            return _mapper.Map<FacultyResponseDto>(updatedFaculty);
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
            var id = dto.FacultyId;
            var faculty = await _facultyRepository.GetByIdAsync(id);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {id} not found.");

            if (dto.Photo == null || dto.Photo.Length == 0)
                throw new ValidationException("Please select a valid photo file to upload.");

            if (dto.Photo.Length > MaxPhotoFileSizeBytes)
                throw new ValidationException("Photo file size exceeds maximum limit of 5 MB.");

            var fileExtension = Path.GetExtension(dto.Photo.FileName).ToLowerInvariant();
            if (!AllowedPhotoExtensions.Contains(fileExtension))
                throw new ValidationException($"Unsupported file format '{fileExtension}'. Allowed formats: JPG, JPEG, PNG.");

            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRootPath, "uploads", "faculty-photos");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // Remove old photo if exists
            if (!string.IsNullOrEmpty(faculty.PhotoPath))
            {
                var oldPhysicalPath = Path.Combine(webRootPath, faculty.PhotoPath.TrimStart('/', '\\'));
                if (File.Exists(oldPhysicalPath))
                {
                    try { File.Delete(oldPhysicalPath); } catch { /* Ignore cleanup errors */ }
                }
            }

            var uniqueFileName = $"faculty_{id}_{Guid.NewGuid():N}{fileExtension}";
            var relativePhotoPath = Path.Combine("uploads", "faculty-photos", uniqueFileName).Replace("\\", "/");
            var physicalPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await dto.Photo.CopyToAsync(stream);
            }

            await _facultyRepository.UpdatePhotoPathAsync(id, relativePhotoPath);
            var updatedFaculty = await _facultyRepository.GetByIdAsync(id);
            return _mapper.Map<FacultyResponseDto>(updatedFaculty);
        }

        public async Task<(string PhysicalPath, string ContentType)> GetPhotoAsync(int id)
        {
            var photoPath = await _facultyRepository.GetPhotoPathAsync(id);
            if (string.IsNullOrEmpty(photoPath))
            {
                throw new NotFoundException($"No photo uploaded for faculty ID {id}.");
            }

            var webRootPath = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var physicalPath = Path.Combine(webRootPath, photoPath.TrimStart('/', '\\'));

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

        private async Task<(Board Board, AcademicLevel Level, AcademicYear Year, Group Group, Section Section, Subject Subject)> ValidateAndResolveMasterEntitiesAsync(
            int boardId, string boardStr,
            int academicLevelId, string levelStr,
            int academicYearId, string yearStr,
            int groupId, string groupStr,
            int sectionId, string sectionStr,
            int subjectId, string subjectStr)
        {
            // 1. Board Resolution
            var board = await _context.Boards.FirstOrDefaultAsync(b => b.BoardId == boardId || (b.BoardName != "" && b.BoardName == boardStr) || (b.BoardCode != "" && b.BoardCode == boardStr));
            if (board == null || !board.IsActive)
            {
                board = await _context.Boards.FirstOrDefaultAsync(b => b.IsActive);
                if (board == null)
                    throw new NotFoundException("Board record not found or inactive.");
            }

            // 2. AcademicLevel Resolution
            var academicLevel = await _context.AcademicLevels.FirstOrDefaultAsync(al => al.AcademicLevelId == academicLevelId || (al.LevelName != "" && al.LevelName == levelStr) || (al.LevelCode != "" && al.LevelCode == levelStr));
            if (academicLevel == null || !academicLevel.IsActive)
            {
                academicLevel = await _context.AcademicLevels.FirstOrDefaultAsync(al => al.IsActive);
                if (academicLevel == null)
                    throw new NotFoundException("Academic Level record not found or inactive.");
            }

            // 3. AcademicYear Resolution
            var academicYear = await _context.AcademicYears.FirstOrDefaultAsync(ay => ay.AcademicYearId == academicYearId || (ay.AcademicYearName != "" && ay.AcademicYearName == yearStr) || (academicYearId > 1900 && ay.AcademicYearName.Contains(academicYearId.ToString())));
            if (academicYear == null)
            {
                academicYear = await _context.AcademicYears.FirstOrDefaultAsync(ay => ay.IsActive);
                if (academicYear == null)
                    throw new NotFoundException("Academic Year record not found.");
            }

            // 4. Group Resolution
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId || (g.GroupName != "" && g.GroupName == groupStr) || (g.GroupCode != "" && g.GroupCode == groupStr));
            if (group == null || !group.IsActive)
            {
                group = await _context.Groups.FirstOrDefaultAsync(g => g.IsActive && (g.GroupName == groupStr || g.GroupCode == groupStr));
                if (group == null)
                    group = await _context.Groups.FirstOrDefaultAsync(g => g.IsActive);

                if (group == null)
                    throw new NotFoundException("Group record not found or inactive.");
            }

            // 5. Section Resolution
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId || (s.SectionName != "" && s.SectionName == sectionStr));
            if (section == null || !section.IsActive)
            {
                section = await _context.Sections.FirstOrDefaultAsync(s => s.IsActive);
                if (section == null)
                    throw new NotFoundException("Section record not found or inactive.");
            }

            // 6. Subject Resolution
            var subject = await _context.Subjects.FirstOrDefaultAsync(sub => sub.SubjectId == subjectId || (sub.SubjectName != "" && sub.SubjectName == subjectStr) || (sub.SubjectCode != "" && sub.SubjectCode == subjectStr));
            if (subject == null)
            {
                subject = await _context.Subjects.FirstOrDefaultAsync(sub => sub.SubjectName == subjectStr || sub.SubjectCode == subjectStr);
                if (subject == null)
                    subject = await _context.Subjects.FirstOrDefaultAsync();

                if (subject == null)
                    throw new NotFoundException("Subject record not found.");
            }

            return (board, academicLevel, academicYear, group, section, subject);
        }

        public async Task<FacultySubjectAllocationResponseDto> AssignSubjectAsync(AssignSubjectDto dto)
        {
            // Verify Faculty Exists
            var faculty = await _facultyRepository.GetByIdAsync(dto.FacultyId);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {dto.FacultyId} not found.");

            // Resolve Master Entities by ID / Name / Code
            var (board, academicLevel, academicYear, group, section, subject) = await ValidateAndResolveMasterEntitiesAsync(
                dto.BoardId, dto.Board,
                dto.AcademicLevelId, dto.AcademicLevel,
                dto.AcademicYearId, dto.AcademicYear,
                dto.GroupId, dto.Group,
                dto.SectionId, dto.Section,
                dto.SubjectId, dto.Subject);

            // Prevent Duplicate Subject Allocation
            if (await _allocationRepository.ExistsAllocationAsync(dto.FacultyId, board.BoardId, academicLevel.AcademicLevelId, academicYear.AcademicYearId, group.GroupId, section.SectionId, subject.SubjectId))
                throw new ConflictException($"Subject '{subject.SubjectName}' is already allocated for Faculty '{faculty.FirstName} {faculty.LastName}' in Section '{section.SectionName}'.");

            var allocation = new FacultySubjectAllocation
            {
                FacultyId = dto.FacultyId,
                BoardId = board.BoardId,
                AcademicLevelId = academicLevel.AcademicLevelId,
                AcademicYearId = academicYear.AcademicYearId,
                GroupId = group.GroupId,
                SectionId = section.SectionId,
                SubjectId = subject.SubjectId
            };

            var createdAllocation = await _allocationRepository.AddAsync(allocation);
            var reloadedAllocation = await _allocationRepository.GetByIdAsync(createdAllocation.Id);
            return _mapper.Map<FacultySubjectAllocationResponseDto>(reloadedAllocation ?? createdAllocation);
        }

        public async Task<FacultySubjectAllocationResponseDto> UpdateSubjectAllocationAsync(int id, UpdateSubjectAllocationDto dto)
        {
            var existingAllocation = await _allocationRepository.GetByIdAsync(id);
            if (existingAllocation == null)
                throw new NotFoundException($"Subject Allocation record with ID {id} not found.");

            // Resolve Master Entities by ID / Name / Code
            var (board, academicLevel, academicYear, group, section, subject) = await ValidateAndResolveMasterEntitiesAsync(
                dto.BoardId, dto.Board,
                dto.AcademicLevelId, dto.AcademicLevel,
                dto.AcademicYearId, dto.AcademicYear,
                dto.GroupId, dto.Group,
                dto.SectionId, dto.Section,
                dto.SubjectId, dto.Subject);

            // Check Duplicate (excluding current allocation id)
            if (await _allocationRepository.ExistsAllocationAsync(existingAllocation.FacultyId, board.BoardId, academicLevel.AcademicLevelId, academicYear.AcademicYearId, group.GroupId, section.SectionId, subject.SubjectId, id))
                throw new ConflictException($"Subject '{subject.SubjectName}' is already allocated in Section '{section.SectionName}'.");

            existingAllocation.BoardId = board.BoardId;
            existingAllocation.AcademicLevelId = academicLevel.AcademicLevelId;
            existingAllocation.AcademicYearId = academicYear.AcademicYearId;
            existingAllocation.GroupId = group.GroupId;
            existingAllocation.SectionId = section.SectionId;
            existingAllocation.SubjectId = subject.SubjectId;

            await _allocationRepository.UpdateAsync(existingAllocation);

            var updatedAllocation = await _allocationRepository.GetByIdAsync(id);
            return _mapper.Map<FacultySubjectAllocationResponseDto>(updatedAllocation ?? existingAllocation);
        }

        public async Task<bool> DeleteSubjectAllocationAsync(int id)
        {
            var existingAllocation = await _allocationRepository.GetByIdAsync(id);
            if (existingAllocation == null)
                throw new NotFoundException($"Subject Allocation record with ID {id} not found.");

            await _allocationRepository.DeleteAsync(existingAllocation);
            return true;
        }

        public async Task<FacultyWorkloadResponseDto?> GetFacultyWorkloadAsync(int facultyId)
        {
            var faculty = await _facultyRepository.GetByIdAsync(facultyId);
            if (faculty == null)
                throw new NotFoundException($"Faculty record with ID {facultyId} not found.");

            var allocations = await _allocationRepository.GetByFacultyIdAsync(facultyId);
            var allocationDtos = _mapper.Map<List<FacultySubjectAllocationResponseDto>>(allocations);

            int totalAssignedSubjects = allocations.Select(a => a.SubjectId).Distinct().Count();
            int totalSections = allocations.Select(a => $"{a.GroupId}_{a.AcademicLevelId}_{a.SectionId}").Distinct().Count();
            int weeklyClasses = allocations.Count * StandardWeeklyClassesPerSubject;
            decimal totalWorkloadHours = weeklyClasses * HoursPerClassPeriod;

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
                TotalWorkloadHours = totalWorkloadHours,
                Allocations = allocationDtos
            };
        }

        public async Task<List<FacultyDropdownDto>> GetFacultyDropdownAsync()
        {
            return await _facultyRepository.GetFacultyDropdownAsync();
        }
    }
}
