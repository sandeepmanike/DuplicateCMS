using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Timetable;
using CollegeManagement.API.Models.Timetable;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Services.Implementations
{
    public class TimetableService : ITimetableService
    {
        private readonly ITimetableRepository _timetableRepository;
        private readonly IPeriodRepository _periodRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly AppDbContext _context;

        public TimetableService(
            ITimetableRepository timetableRepository,
            IPeriodRepository periodRepository,
            IRoomRepository roomRepository,
            AppDbContext context)
        {
            _timetableRepository = timetableRepository;
            _periodRepository = periodRepository;
            _roomRepository = roomRepository;
            _context = context;
        }

        private async Task<TimetableResponseDto?> EnrichDtoAsync(TimetableResponseDto? dto)
        {
            if (dto == null) return null;
            var entity = await _context.Timetables.FirstOrDefaultAsync(t => t.Id == dto.Id);
            if (entity != null)
            {
                dto.ApprovalStatus = (int)entity.ApprovalStatus;
                dto.ApprovalStatusName = entity.ApprovalStatus.ToString();
                dto.IsPublished = entity.IsPublished;
            }
            else
            {
                dto.ApprovalStatusName = ((TimetableApprovalStatus)dto.ApprovalStatus).ToString();
            }
            return dto;
        }

        private async Task<IEnumerable<TimetableResponseDto>> EnrichDtosAsync(IEnumerable<TimetableResponseDto> dtos)
        {
            var list = dtos.ToList();
            if (!list.Any()) return list;

            var ids = list.Select(d => d.Id).ToList();
            var entities = await _context.Timetables.Where(t => ids.Contains(t.Id)).ToDictionaryAsync(t => t.Id);

            foreach (var dto in list)
            {
                if (entities.TryGetValue(dto.Id, out var entity))
                {
                    dto.ApprovalStatus = (int)entity.ApprovalStatus;
                    dto.ApprovalStatusName = entity.ApprovalStatus.ToString();
                    dto.IsPublished = entity.IsPublished;
                }
                else
                {
                    dto.ApprovalStatusName = ((TimetableApprovalStatus)dto.ApprovalStatus).ToString();
                }
            }
            return list;
        }

        public async Task<TimetableResponseDto?> GetByIdAsync(int id)
        {
            var dto = await _timetableRepository.GetByIdAsync(id);
            return await EnrichDtoAsync(dto);
        }

        public async Task<(IEnumerable<TimetableResponseDto> Items, int TotalCount)> GetPagedAsync(TimetableQueryParams queryParams)
        {
            var (items, totalCount) = await _timetableRepository.GetPagedAsync(queryParams);
            var enriched = await EnrichDtosAsync(items);
            return (enriched, totalCount);
        }

        public async Task<IEnumerable<TimetableResponseDto>> GetFacultyTimetableAsync(int facultyId, int? academicYearId = null)
        {
            var dtos = await _timetableRepository.GetByFacultyIdAsync(facultyId, academicYearId);
            return await EnrichDtosAsync(dtos);
        }

        public async Task<IEnumerable<TimetableResponseDto>> GetSectionTimetableAsync(int sectionId, int? academicYearId = null, bool? isPublished = null)
        {
            var dtos = await _timetableRepository.GetBySectionIdAsync(sectionId, academicYearId, isPublished);
            return await EnrichDtosAsync(dtos);
        }

        public async Task<IEnumerable<TimetableResponseDto>> GetStudentTimetableAsync(int studentId)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                throw new KeyNotFoundException($"Student with ID {studentId} not found.");
            }

            var section = await _context.Sections.FirstOrDefaultAsync(sec => sec.SectionId == student.SectionId || sec.SectionId == student.GroupId);
            if (section == null)
            {
                section = await _context.Sections.FirstOrDefaultAsync();
            }

            int sectionId = section?.SectionId ?? 1;
            var dtos = await _timetableRepository.GetBySectionIdAsync(sectionId, student.AcademicYearId, isPublished: true);
            return await EnrichDtosAsync(dtos);
        }

        public async Task<IEnumerable<AllocatedFacultyDto>> GetAllocatedFacultiesAsync(int? boardId, int? academicLevelId, int? academicYearId, int? groupId, int? sectionId, int? subjectId)
        {
            if (subjectId.HasValue && subjectId.Value > 0)
            {
                var allocatedFacultyIds = await _context.FacultySubjectAllocations
                    .Where(fsa => fsa.SubjectId == subjectId.Value)
                    .Select(fsa => fsa.FacultyId)
                    .Distinct()
                    .ToListAsync();

                var faculties = await _context.Faculties
                    .Where(f => allocatedFacultyIds.Contains(f.Id) && !f.IsDeleted && f.Status.ToLower() == "active" && f.FacultyType.ToLower() != "non-teaching")
                    .Select(f => new AllocatedFacultyDto
                    {
                        FacultyId = f.Id,
                        FacultyName = $"{f.FirstName} {f.LastName}".Trim()
                    })
                    .ToListAsync();

                return faculties;
            }

            return await _timetableRepository.GetAllocatedFacultiesAsync(boardId, academicLevelId, academicYearId, groupId, sectionId, subjectId);
        }

        public async Task<TimetableResponseDto> CreateAsync(CreateTimetableDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            dto.IsPublished = false;

            if (dto.BoardId <= 0) dto.BoardId = (await _context.Boards.FirstOrDefaultAsync(b => b.IsActive))?.BoardId ?? 1;
            if (dto.AcademicLevelId <= 0) dto.AcademicLevelId = (await _context.AcademicLevels.FirstOrDefaultAsync(al => al.IsActive))?.AcademicLevelId ?? 1;
            if (dto.AcademicYearId <= 0) dto.AcademicYearId = (await _context.AcademicYears.FirstOrDefaultAsync(ay => ay.IsActive))?.AcademicYearId ?? 1;
            if (dto.GroupId <= 0) dto.GroupId = (await _context.Groups.FirstOrDefaultAsync(g => g.IsActive))?.GroupId ?? 1;
            if (dto.SectionId <= 0) dto.SectionId = (await _context.Sections.FirstOrDefaultAsync(s => s.IsActive))?.SectionId ?? 1;
            if (dto.SubjectId <= 0) dto.SubjectId = (await _context.Subjects.FirstOrDefaultAsync())?.SubjectId ?? 1;

            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == dto.SectionId);
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId);

            if (subject != null && subject.Theory && section?.RoomId.HasValue == true && section.RoomId.Value > 0)
            {
                dto.RoomId = section.RoomId.Value;
            }
            else if (dto.RoomId <= 0 && section?.RoomId.HasValue == true && section.RoomId.Value > 0)
            {
                dto.RoomId = section.RoomId.Value;
            }
            else if (dto.RoomId <= 0)
            {
                dto.RoomId = (await _roomRepository.GetAllAsync())?.FirstOrDefault(r => r.IsActive)?.RoomId ?? 1;
            }

            if (dto.FacultyId <= 0) dto.FacultyId = (await _context.Faculties.FirstOrDefaultAsync(f => !f.IsDeleted && f.Status.ToLower() == "active" && f.FacultyType.ToLower() != "non-teaching"))?.Id ?? 1;
            if (dto.PeriodId <= 0) dto.PeriodId = (await _periodRepository.GetAllAsync())?.FirstOrDefault(p => !p.IsBreak)?.PeriodId ?? 1;
            if (dto.DayOfWeek <= 0) dto.DayOfWeek = 1;

            await ValidateSlotAndConflictsAsync(dto.AcademicYearId, dto.SectionId, dto.FacultyId, dto.RoomId, dto.DayOfWeek, dto.PeriodId, dto.SubjectId, dto.BoardId, dto.GroupId, dto.AcademicLevelId, excludeId: null);

            int newId = await _timetableRepository.AddAsync(dto);
            var result = await _timetableRepository.GetByIdAsync(newId);
            return await EnrichDtoAsync(result) ?? throw new InvalidOperationException("Failed to retrieve created timetable slot.");
        }

        public async Task<TimetableResponseDto?> UpdateAsync(int id, UpdateTimetableDto dto)
        {
            var existing = await _context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null) return null;

            if (existing.IsPublished || existing.ApprovalStatus != TimetableApprovalStatus.Draft)
            {
                throw new InvalidOperationException($"Cannot edit slot ID {id}: Only DRAFT timetable slots can be edited. Current status: {existing.ApprovalStatus}.");
            }

            dto.IsPublished = false;

            if (dto.BoardId <= 0) dto.BoardId = existing.BoardId;
            if (dto.AcademicLevelId <= 0) dto.AcademicLevelId = existing.AcademicLevelId;
            if (dto.AcademicYearId <= 0) dto.AcademicYearId = existing.AcademicYearId;
            if (dto.GroupId <= 0) dto.GroupId = existing.GroupId;
            if (dto.SectionId <= 0) dto.SectionId = existing.SectionId;
            if (dto.SubjectId <= 0) dto.SubjectId = existing.SubjectId;
            if (dto.FacultyId <= 0) dto.FacultyId = existing.FacultyId;
            if (dto.PeriodId <= 0) dto.PeriodId = existing.PeriodId;
            if (dto.DayOfWeek <= 0) dto.DayOfWeek = existing.DayOfWeek > 0 ? existing.DayOfWeek : 1;

            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == dto.SectionId);
            var subject = await _context.Subjects.FirstOrDefaultAsync(s => s.SubjectId == dto.SubjectId);

            if (subject != null && subject.Theory && section?.RoomId.HasValue == true && section.RoomId.Value > 0)
            {
                dto.RoomId = section.RoomId.Value;
            }
            else if (dto.RoomId <= 0 && section?.RoomId.HasValue == true && section.RoomId.Value > 0)
            {
                dto.RoomId = section.RoomId.Value;
            }
            else if (dto.RoomId <= 0)
            {
                dto.RoomId = existing.RoomId;
            }

            await ValidateSlotAndConflictsAsync(dto.AcademicYearId, dto.SectionId, dto.FacultyId, dto.RoomId, dto.DayOfWeek, dto.PeriodId, dto.SubjectId, dto.BoardId, dto.GroupId, dto.AcademicLevelId, excludeId: id);

            await _timetableRepository.UpdateAsync(id, dto);
            var result = await _timetableRepository.GetByIdAsync(id);
            return await EnrichDtoAsync(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
            if (existing == null) return false;

            if (existing.IsPublished || existing.ApprovalStatus != TimetableApprovalStatus.Draft)
            {
                throw new InvalidOperationException($"Cannot delete slot ID {id}: Only DRAFT timetable slots can be deleted. Current status: {existing.ApprovalStatus}.");
            }

            await _timetableRepository.DeleteAsync(id);
            return true;
        }

        public async Task<bool> TogglePublishSlotAsync(int id, bool isPublished)
        {
            var slot = await _context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
            if (slot == null) return false;

            if (isPublished)
            {
                if (slot.ApprovalStatus != TimetableApprovalStatus.Approved)
                {
                    throw new InvalidOperationException($"Cannot publish slot ID {id}: Slot must be in APPROVED state before publishing. Current status: {slot.ApprovalStatus}.");
                }

                slot.ApprovalStatus = TimetableApprovalStatus.Published;
                slot.IsPublished = true;
            }
            else
            {
                slot.ApprovalStatus = TimetableApprovalStatus.Approved;
                slot.IsPublished = false;
            }

            slot.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PublishSectionTimetableAsync(int sectionId, int academicYearId, bool isPublished)
        {
            if (academicYearId <= 0)
            {
                var sec = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);
                academicYearId = sec != null && sec.AcademicYearId > 0
                    ? sec.AcademicYearId
                    : await _context.Timetables.Where(t => t.SectionId == sectionId).Select(t => t.AcademicYearId).FirstOrDefaultAsync();
            }

            var sectionSlots = await _context.Timetables
                .Where(t => t.SectionId == sectionId && (academicYearId <= 0 || t.AcademicYearId == academicYearId))
                .ToListAsync();

            if (!sectionSlots.Any())
            {
                throw new KeyNotFoundException($"No timetable slots found for Section ID {sectionId}.");
            }

            if (isPublished)
            {
                var unapprovedDrafts = sectionSlots.Where(t => t.ApprovalStatus == TimetableApprovalStatus.Draft).ToList();
                if (unapprovedDrafts.Count > 0)
                {
                    throw new InvalidOperationException($"Cannot publish section timetable: {unapprovedDrafts.Count} slot(s) are still in DRAFT state. All slots must be APPROVED before publishing.");
                }

                var validation = await ValidateSectionTimetableAsync(sectionId, academicYearId);
                if (!validation.IsValid)
                {
                    throw new InvalidOperationException($"Cannot publish section timetable: Validation errors found. First error: {validation.Errors.First().Message}");
                }

                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        foreach (var slot in sectionSlots)
                        {
                            slot.ApprovalStatus = TimetableApprovalStatus.Published;
                            slot.IsPublished = true;
                            slot.UpdatedAt = DateTime.UtcNow;
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }
            else
            {
                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync();
                    try
                    {
                        foreach (var slot in sectionSlots)
                        {
                            slot.ApprovalStatus = TimetableApprovalStatus.Approved;
                            slot.IsPublished = false;
                            slot.UpdatedAt = DateTime.UtcNow;
                        }
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                    }
                    catch
                    {
                        await transaction.RollbackAsync();
                        throw;
                    }
                });
            }

            return true;
        }

        public async Task<bool> CopyTimetableAsync(CopyTimetableDto dto)
        {
            var sourceSlots = await _timetableRepository.GetBySectionIdAsync(dto.SourceSectionId, dto.SourceAcademicYearId);
            if (!sourceSlots.Any())
            {
                throw new InvalidOperationException("No timetable slots found in source section to copy.");
            }

            await _timetableRepository.CopySectionTimetableAsync(dto);
            return true;
        }

        public async Task<ValidateTimetableResultDto> ValidateSectionTimetableAsync(int sectionId, int academicYearId)
        {
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);
            if (section == null)
            {
                throw new KeyNotFoundException($"Section with ID {sectionId} not found.");
            }

            if (academicYearId <= 0)
            {
                academicYearId = section.AcademicYearId > 0
                    ? section.AcademicYearId
                    : await _context.Timetables.Where(t => t.SectionId == sectionId).Select(t => t.AcademicYearId).FirstOrDefaultAsync();
            }

            var slots = await _context.Timetables
                .Where(t => t.SectionId == sectionId && (academicYearId <= 0 || t.AcademicYearId == academicYearId))
                .ToListAsync();

            var result = new ValidateTimetableResultDto
            {
                SectionId = sectionId,
                SectionName = section.SectionName,
                AcademicYearId = academicYearId,
                TotalSlots = slots.Count,
                IsValid = true
            };

            if (slots.Count == 0)
            {
                result.Warnings.Add(new TimetableValidationErrorDto
                {
                    Message = "No timetable slots found for this section in the specified academic year.",
                    Code = "NO_SLOTS"
                });
                return result;
            }

            var periodIds = slots.Select(s => s.PeriodId).Distinct().ToList();
            var periodsDict = await _context.Periods.Where(p => periodIds.Contains(p.PeriodId)).ToDictionaryAsync(p => p.PeriodId);

            var subjectIds = slots.Select(s => s.SubjectId).Distinct().ToList();
            var subjectsDict = await _context.Subjects.Where(s => subjectIds.Contains(s.SubjectId)).ToDictionaryAsync(s => s.SubjectId);

            var facultyIds = slots.Select(s => s.FacultyId).Distinct().ToList();
            var facultiesDict = await _context.Faculties.Where(f => facultyIds.Contains(f.Id)).ToDictionaryAsync(f => f.Id);

            var roomIds = slots.Select(s => s.RoomId).Distinct().ToList();
            var roomsDict = await _context.Rooms.Where(r => roomIds.Contains(r.RoomId)).ToDictionaryAsync(r => r.RoomId);

            var allocationsSet = (await _context.FacultySubjectAllocations
                .Where(fsa => subjectIds.Contains(fsa.SubjectId) && facultyIds.Contains(fsa.FacultyId))
                .Select(fsa => new { fsa.FacultyId, fsa.SubjectId })
                .ToListAsync())
                .Select(a => (a.FacultyId, a.SubjectId))
                .ToHashSet();

            var allYearTimetables = await _context.Timetables
                .Where(t => t.AcademicYearId == academicYearId)
                .ToListAsync();

            var sectionSlotCheck = new HashSet<(int Day, int PeriodId)>();

            foreach (var slot in slots)
            {
                string dayName = GetDayName(slot.DayOfWeek);
                string periodName = periodsDict.TryGetValue(slot.PeriodId, out var p) ? p.PeriodName : $"Period {slot.PeriodId}";
                string subjectName = subjectsDict.TryGetValue(slot.SubjectId, out var sub) ? sub.SubjectName : $"Subject {slot.SubjectId}";
                string facultyName = facultiesDict.TryGetValue(slot.FacultyId, out var f) ? $"{f.FirstName} {f.LastName}".Trim() : $"Faculty {slot.FacultyId}";
                string roomName = roomsDict.TryGetValue(slot.RoomId, out var r) ? r.RoomNumber : $"Room {slot.RoomId}";

                void AddError(string msg, string code)
                {
                    result.IsValid = false;
                    result.Errors.Add(new TimetableValidationErrorDto
                    {
                        TimetableId = slot.Id,
                        DayOfWeek = slot.DayOfWeek,
                        DayName = dayName,
                        PeriodId = slot.PeriodId,
                        PeriodName = periodName,
                        SubjectId = slot.SubjectId,
                        SubjectName = subjectName,
                        FacultyId = slot.FacultyId,
                        FacultyName = facultyName,
                        RoomId = slot.RoomId,
                        RoomName = roomName,
                        Message = msg,
                        Code = code
                    });
                }

                // 1. Period Active & Non-Break check
                if (!periodsDict.TryGetValue(slot.PeriodId, out var periodObj) || !periodObj.IsActive)
                {
                    AddError($"Period '{periodName}' is inactive or invalid.", "INACTIVE_PERIOD");
                }
                else if (periodObj.IsBreak)
                {
                    AddError($"Period '{periodName}' is designated as a Break period. Classes cannot be scheduled during break times.", "BREAK_PERIOD_CONFLICT");
                }

                // 2. Active Non-deleted Teaching Faculty check
                if (!facultiesDict.TryGetValue(slot.FacultyId, out var facultyObj) || facultyObj.IsDeleted || facultyObj.Status.ToLower() != "active" || facultyObj.FacultyType.ToLower() == "non-teaching")
                {
                    AddError($"Faculty '{facultyName}' is inactive, non-teaching, or deleted.", "INACTIVE_FACULTY");
                }

                // 3. FacultySubjectAllocation eligibility check
                if (!allocationsSet.Contains((slot.FacultyId, slot.SubjectId)))
                {
                    AddError($"Faculty '{facultyName}' is not allocated to teach subject '{subjectName}'.", "UNALLOCATED_FACULTY");
                }

                // 4. Duplicate Section Slot check
                if (sectionSlotCheck.Contains((slot.DayOfWeek, slot.PeriodId)))
                {
                    AddError($"Section conflict: Duplicate slot in Section '{section.SectionName}' on {dayName} during {periodName}.", "SECTION_CLASH");
                }
                else
                {
                    sectionSlotCheck.Add((slot.DayOfWeek, slot.PeriodId));
                }

                // 5. Cross-Section Faculty Conflict check
                var facClash = allYearTimetables.FirstOrDefault(t => t.FacultyId == slot.FacultyId && t.DayOfWeek == slot.DayOfWeek && t.PeriodId == slot.PeriodId && t.SectionId != sectionId);
                if (facClash != null)
                {
                    var otherSec = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == facClash.SectionId);
                    string otherSecName = otherSec?.SectionName ?? $"Section {facClash.SectionId}";
                    AddError($"Faculty conflict: Faculty '{facultyName}' is already assigned to '{otherSecName}' on {dayName} during {periodName}.", "FACULTY_CLASH");
                }

                // 6. Cross-Section Room Conflict check
                var roomClash = allYearTimetables.FirstOrDefault(t => t.RoomId == slot.RoomId && t.DayOfWeek == slot.DayOfWeek && t.PeriodId == slot.PeriodId && t.SectionId != sectionId);
                if (roomClash != null)
                {
                    var otherSec = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == roomClash.SectionId);
                    string otherSecName = otherSec?.SectionName ?? $"Section {roomClash.SectionId}";
                    AddError($"Room conflict: Room '{roomName}' is already booked for '{otherSecName}' on {dayName} during {periodName}.", "ROOM_CLASH");
                }
            }

            return result;
        }

        public async Task<ApproveTimetableResultDto> ApproveSectionTimetableAsync(int sectionId, int academicYearId)
        {
            var section = await _context.Sections.FirstOrDefaultAsync(s => s.SectionId == sectionId);
            if (section == null)
            {
                throw new KeyNotFoundException($"Section with ID {sectionId} not found.");
            }

            if (academicYearId <= 0)
            {
                academicYearId = section.AcademicYearId > 0
                    ? section.AcademicYearId
                    : await _context.Timetables.Where(t => t.SectionId == sectionId).Select(t => t.AcademicYearId).FirstOrDefaultAsync();
            }

            var allSectionSlots = await _context.Timetables
                .Where(t => t.SectionId == sectionId && (academicYearId <= 0 || t.AcademicYearId == academicYearId))
                .ToListAsync();

            if (academicYearId <= 0 && allSectionSlots.Count > 0)
            {
                academicYearId = allSectionSlots.First().AcademicYearId;
            }

            var draftSlots = allSectionSlots.Where(t => t.ApprovalStatus == TimetableApprovalStatus.Draft).ToList();

            if (draftSlots.Count == 0)
            {
                throw new InvalidOperationException($"No DRAFT timetable slots found to approve for Section '{section.SectionName}' (ID: {sectionId}).");
            }

            var validationResult = await ValidateSectionTimetableAsync(sectionId, academicYearId);
            if (!validationResult.IsValid)
            {
                var firstErr = validationResult.Errors.First().Message;
                throw new InvalidOperationException($"Cannot approve section timetable: Batch validation failed with {validationResult.Errors.Count} error(s). First error: {firstErr}");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    foreach (var slot in draftSlots)
                    {
                        slot.ApprovalStatus = TimetableApprovalStatus.Approved;
                        slot.IsPublished = false;
                        slot.UpdatedAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            var approvedDtos = await GetSectionTimetableAsync(sectionId, academicYearId);

            return new ApproveTimetableResultDto
            {
                IsSuccess = true,
                Message = $"Successfully approved {draftSlots.Count} timetable slot(s) for Section '{section.SectionName}'.",
                SectionId = sectionId,
                AcademicYearId = academicYearId,
                TotalSlotsApproved = draftSlots.Count,
                ApprovedSlots = approvedDtos.ToList()
            };
        }

        private static string GetDayName(int dayOfWeek) => dayOfWeek switch
        {
            1 => "Monday",
            2 => "Tuesday",
            3 => "Wednesday",
            4 => "Thursday",
            5 => "Friday",
            6 => "Saturday",
            7 => "Sunday",
            _ => $"Day {dayOfWeek}"
        };

        private async Task ValidateSlotAndConflictsAsync(int academicYearId, int sectionId, int facultyId, int roomId, int dayOfWeek, int periodId, int subjectId, int boardId, int groupId, int academicLevelId, int? excludeId)
        {
            var board = await _context.Boards.FindAsync(boardId) 
                        ?? await _context.Boards.FirstOrDefaultAsync(b => b.IsActive);
            if (board == null) throw new InvalidOperationException("Please select a valid Board.");

            var academicLevel = await _context.AcademicLevels.FindAsync(academicLevelId) 
                                ?? await _context.AcademicLevels.FirstOrDefaultAsync(al => al.IsActive);
            if (academicLevel == null) throw new InvalidOperationException("Please select a valid Academic Level.");

            var academicYear = await _context.AcademicYears.FindAsync(academicYearId) 
                               ?? await _context.AcademicYears.FirstOrDefaultAsync(ay => ay.IsActive);
            if (academicYear == null) throw new InvalidOperationException("Please select a valid Academic Year.");

            var group = await _context.Groups.FindAsync(groupId) 
                        ?? await _context.Groups.FirstOrDefaultAsync(g => g.IsActive);
            if (group == null) throw new InvalidOperationException("Please select a valid Group.");

            var section = await _context.Sections.FindAsync(sectionId)
                          ?? await _context.Sections.FirstOrDefaultAsync(s => s.IsActive);
            if (section == null) throw new InvalidOperationException("Please select a valid Section.");

            var subject = await _context.Subjects.FindAsync(subjectId)
                         ?? await _context.Subjects.FirstOrDefaultAsync();
            if (subject == null) throw new InvalidOperationException("Please select a valid Subject.");

            var faculty = await _context.Faculties.FindAsync(facultyId);
            if (faculty == null || faculty.IsDeleted || faculty.Status.ToLower() != "active" || faculty.FacultyType.ToLower() == "non-teaching")
            {
                throw new InvalidOperationException($"Faculty '{(faculty != null ? $"{faculty.FirstName} {faculty.LastName}" : facultyId.ToString())}' is inactive or non-teaching and cannot be assigned.");
            }

            var period = await _periodRepository.GetByIdAsync(periodId);
            if (period == null)
            {
                var allPeriods = await _periodRepository.GetAllAsync();
                period = allPeriods.FirstOrDefault(p => !p.IsBreak);
                if (period == null) throw new InvalidOperationException("Please select a valid Period.");
            }
            if (period.IsBreak)
            {
                throw new InvalidOperationException($"Period '{period.PeriodName}' is designated as a Break period. Classes cannot be scheduled during break times.");
            }

            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null || !room.IsActive)
            {
                var allRooms = await _roomRepository.GetAllAsync();
                room = allRooms.FirstOrDefault(r => r.IsActive);
                if (room == null) throw new InvalidOperationException("Please select a valid Room.");
            }

            var isAllocated = await _context.FacultySubjectAllocations
                .AnyAsync(fsa => fsa.FacultyId == facultyId && fsa.SubjectId == subjectId);
            if (!isAllocated)
            {
                throw new InvalidOperationException($"Faculty '{faculty.FirstName} {faculty.LastName}' is not allocated to teach subject '{subject.SubjectName}'.");
            }

            string dayName = GetDayName(dayOfWeek);

            bool sectionConflict = await _timetableRepository.HasSectionSlotConflictAsync(academicYearId, sectionId, dayOfWeek, periodId, excludeId);
            if (sectionConflict)
            {
                throw new InvalidOperationException($"Section conflict: Section '{section.SectionName}' already has a slot scheduled on {dayName} during Period {period.PeriodName}.");
            }

            bool facultyConflict = await _timetableRepository.HasFacultySlotConflictAsync(academicYearId, facultyId, dayOfWeek, periodId, excludeId);
            if (facultyConflict)
            {
                throw new InvalidOperationException($"Faculty {faculty.FirstName} {faculty.LastName} is already assigned to another section on {dayName} Period {period.PeriodName}.");
            }

            bool roomConflict = await _timetableRepository.HasRoomSlotConflictAsync(academicYearId, roomId, dayOfWeek, periodId, excludeId);
            if (roomConflict)
            {
                throw new InvalidOperationException($"Room conflict: Room '{room.RoomCode}' ({room.RoomName}) is already booked for another section on {dayName} during Period {period.PeriodName}.");
            }
        }

        public async Task<GenerateTimetableResultDto> GenerateTheoryTimetableAsync(GenerateTimetableRequestDto dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            var year = await _context.AcademicYears.FirstOrDefaultAsync(y => y.AcademicYearId == dto.AcademicYearId);
            if (year == null)
                throw new KeyNotFoundException($"Academic Year with ID {dto.AcademicYearId} not found.");

            var board = await _context.Boards.FirstOrDefaultAsync(b => b.BoardId == dto.BoardId);
            if (board == null)
                throw new KeyNotFoundException($"Board with ID {dto.BoardId} not found.");

            var level = await _context.AcademicLevels.FirstOrDefaultAsync(l => l.AcademicLevelId == dto.AcademicLevelId);
            if (level == null)
                throw new KeyNotFoundException($"Academic Level with ID {dto.AcademicLevelId} not found.");

            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == dto.GroupId);
            if (group == null)
                throw new KeyNotFoundException($"Group with ID {dto.GroupId} not found.");

            var targetSections = await _context.Sections
                .Where(sec => dto.SectionIds.Contains(sec.SectionId) && sec.IsActive)
                .ToListAsync();

            if (targetSections.Count != dto.SectionIds.Distinct().Count())
            {
                var foundIds = targetSections.Select(s => s.SectionId).ToHashSet();
                var missingIds = dto.SectionIds.Where(id => !foundIds.Contains(id)).ToList();
                throw new KeyNotFoundException($"Active Section(s) with ID(s) [{string.Join(", ", missingIds)}] not found.");
            }

            var protectedSectionSlots = await _context.Timetables
                .Where(t => dto.SectionIds.Contains(t.SectionId) && t.AcademicYearId == dto.AcademicYearId && (t.IsPublished || t.ApprovalStatus != TimetableApprovalStatus.Draft))
                .ToListAsync();

            if (protectedSectionSlots.Count > 0)
            {
                var protectedSecIds = protectedSectionSlots.Select(s => s.SectionId).Distinct().ToList();
                var protectedSecNames = targetSections.Where(s => protectedSecIds.Contains(s.SectionId)).Select(s => s.SectionName);
                throw new InvalidOperationException($"Cannot auto-generate timetable: Section(s) [{string.Join(", ", protectedSecNames)}] contain APPROVED or PUBLISHED slots. Automatic generation can only replace DRAFT sections.");
            }

            foreach (var sec in targetSections)
            {
                if (sec.GroupId != dto.GroupId && sec.GroupId.HasValue && sec.GroupId.Value != dto.GroupId)
                {
                    throw new InvalidOperationException($"Section '{sec.SectionName}' (ID: {sec.SectionId}) does not belong to Group ID {dto.GroupId}.");
                }
                if (!sec.RoomId.HasValue || sec.RoomId.Value <= 0)
                {
                    throw new InvalidOperationException($"Section '{sec.SectionName}' (ID: {sec.SectionId}) does not have a default Room assigned.");
                }
            }

            var roomIds = targetSections.Select(s => s.RoomId!.Value).Distinct().ToList();
            var roomsDict = await _context.Rooms
                .Where(r => roomIds.Contains(r.RoomId))
                .ToDictionaryAsync(r => r.RoomId);

            foreach (var sec in targetSections)
            {
                if (!roomsDict.ContainsKey(sec.RoomId!.Value))
                {
                    throw new InvalidOperationException($"Room ID {sec.RoomId.Value} assigned to Section '{sec.SectionName}' was not found in the Rooms master.");
                }
            }

            var reqSubjectIds = dto.SubjectRequirements.Select(r => r.SubjectId).Distinct().ToList();
            var dbSubjects = await _context.Subjects
                .Where(s => reqSubjectIds.Contains(s.SubjectId))
                .ToDictionaryAsync(s => s.SubjectId);

            foreach (var req in dto.SubjectRequirements)
            {
                if (!dbSubjects.TryGetValue(req.SubjectId, out var subject))
                {
                    throw new KeyNotFoundException($"Subject with ID {req.SubjectId} not found.");
                }
                if (subject.GroupId != dto.GroupId)
                {
                    throw new InvalidOperationException($"Subject '{subject.SubjectName}' (ID: {req.SubjectId}) does not belong to Group ID {dto.GroupId}.");
                }
                if (!subject.Theory)
                {
                    throw new InvalidOperationException($"Subject '{subject.SubjectName}' (ID: {req.SubjectId}) is not a Theory subject. Automatic generation is theory-only.");
                }
            }

            var activePeriods = await _context.Periods
                .Where(p => p.IsActive && !p.IsBreak)
                .OrderBy(p => p.DisplayOrder)
                .ToListAsync();

            if (activePeriods.Count == 0)
            {
                throw new InvalidOperationException("No active non-break periods configured in the system.");
            }

            var workingDays = dto.WorkingDays.OrderBy(d => d).ToList();
            int totalSlotsPerSection = workingDays.Count * activePeriods.Count;
            int totalRequiredPeriodsPerSection = dto.SubjectRequirements.Sum(r => r.WeeklyPeriods);

            if (totalRequiredPeriodsPerSection > totalSlotsPerSection)
            {
                throw new InvalidOperationException($"Total requested weekly theory periods ({totalRequiredPeriodsPerSection}) per section exceeds total available slots ({totalSlotsPerSection}) across {workingDays.Count} working days.");
            }

            var subjectEligibleFaculties = new Dictionary<int, List<Models.Faculty.Faculty>>();
            foreach (var req in dto.SubjectRequirements)
            {
                var subjectId = req.SubjectId;
                var eligibleFacultyIds = await _context.FacultySubjectAllocations
                    .Where(fsa => fsa.SubjectId == subjectId)
                    .Select(fsa => fsa.FacultyId)
                    .Distinct()
                    .ToListAsync();

                var faculties = await _context.Faculties
                    .Where(f => eligibleFacultyIds.Contains(f.Id) && !f.IsDeleted && f.Status.ToLower() == "active" && f.FacultyType.ToLower() != "non-teaching")
                    .ToListAsync();

                if (faculties.Count == 0)
                {
                    var sName = dbSubjects[subjectId].SubjectName;
                    throw new InvalidOperationException($"No eligible active teaching faculty found allocated for Subject '{sName}' (ID: {subjectId}).");
                }

                subjectEligibleFaculties[subjectId] = faculties;
            }

            var existingTimetables = await _context.Timetables
                .Where(t => t.AcademicYearId == dto.AcademicYearId)
                .ToListAsync();

            var protectedSlots = existingTimetables.Where(t => t.IsPublished || t.ApprovalStatus != TimetableApprovalStatus.Draft).ToList();
            var otherDraftSlots = existingTimetables.Where(t => t.ApprovalStatus == TimetableApprovalStatus.Draft && !dto.SectionIds.Contains(t.SectionId)).ToList();

            var occupiedSection = new HashSet<(int SectionId, int Day, int PeriodId)>();
            var occupiedFaculty = new HashSet<(int FacultyId, int Day, int PeriodId)>();
            var occupiedRoom = new HashSet<(int RoomId, int Day, int PeriodId)>();

            void OccupySlot(int secId, int facId, int rId, int day, int pId)
            {
                occupiedSection.Add((secId, day, pId));
                occupiedFaculty.Add((facId, day, pId));
                occupiedRoom.Add((rId, day, pId));
            }

            foreach (var slot in protectedSlots.Concat(otherDraftSlots))
            {
                OccupySlot(slot.SectionId, slot.FacultyId, slot.RoomId, slot.DayOfWeek, slot.PeriodId);
            }

            var orderedRequirements = dto.SubjectRequirements
                .OrderBy(r => subjectEligibleFaculties[r.SubjectId].Count)
                .ThenByDescending(r => r.WeeklyPeriods)
                .ToList();

            var generatedDraftEntities = new List<CollegeManagement.API.Models.Timetable.Timetable>();
            var warnings = new List<UnassignedSlotWarningDto>();
            var facultyPeriodCount = new Dictionary<int, int>();

            foreach (var sec in targetSections)
            {
                int defaultRoomId = sec.RoomId!.Value;
                var sectionDaySubjectCount = new Dictionary<(int Day, int SubjectId), int>();

                foreach (var req in orderedRequirements)
                {
                    var subject = dbSubjects[req.SubjectId];
                    var eligibleFaculties = subjectEligibleFaculties[req.SubjectId];
                    int requiredPeriods = req.WeeklyPeriods;
                    int placedCount = 0;

                    for (int occurrence = 0; occurrence < requiredPeriods; occurrence++)
                    {
                        var candidates = new List<(int Day, int PeriodId, Models.Faculty.Faculty Faculty, int Score)>();

                        foreach (int day in workingDays)
                        {
                            int curDaySubCount = sectionDaySubjectCount.GetValueOrDefault((day, subject.SubjectId), 0);

                            foreach (var period in activePeriods)
                            {
                                if (occupiedSection.Contains((sec.SectionId, day, period.PeriodId))) continue;
                                if (occupiedRoom.Contains((defaultRoomId, day, period.PeriodId))) continue;

                                foreach (var fac in eligibleFaculties)
                                {
                                    if (occupiedFaculty.Contains((fac.Id, day, period.PeriodId))) continue;

                                    int score = 1000;

                                    if (curDaySubCount == 0) score += 500;
                                    else if (curDaySubCount == 1) score += 100;
                                    else score -= (curDaySubCount * 200);

                                    int facLoad = facultyPeriodCount.GetValueOrDefault(fac.Id, 0);
                                    score -= (facLoad * 10);

                                    candidates.Add((day, period.PeriodId, fac, score));
                                }
                            }
                        }

                        if (candidates.Count > 0)
                        {
                            var best = candidates.OrderByDescending(c => c.Score).First();

                            OccupySlot(sec.SectionId, best.Faculty.Id, defaultRoomId, best.Day, best.PeriodId);
                            sectionDaySubjectCount[(best.Day, subject.SubjectId)] = sectionDaySubjectCount.GetValueOrDefault((best.Day, subject.SubjectId), 0) + 1;
                            facultyPeriodCount[best.Faculty.Id] = facultyPeriodCount.GetValueOrDefault(best.Faculty.Id, 0) + 1;

                            var newSlot = new CollegeManagement.API.Models.Timetable.Timetable
                            {
                                BoardId = dto.BoardId,
                                AcademicLevelId = dto.AcademicLevelId,
                                AcademicYearId = dto.AcademicYearId,
                                GroupId = dto.GroupId,
                                SectionId = sec.SectionId,
                                DayOfWeek = best.Day,
                                PeriodId = best.PeriodId,
                                SubjectId = subject.SubjectId,
                                FacultyId = best.Faculty.Id,
                                RoomId = defaultRoomId,
                                IsPublished = false,
                                ApprovalStatus = TimetableApprovalStatus.Draft,
                                Remarks = "Auto-generated theory slot",
                                CreatedAt = DateTime.UtcNow
                            };

                            generatedDraftEntities.Add(newSlot);
                            placedCount++;
                        }
                        else
                        {
                            int remainingUnassigned = requiredPeriods - placedCount;
                            warnings.Add(new UnassignedSlotWarningDto
                            {
                                SectionId = sec.SectionId,
                                SectionName = sec.SectionName,
                                SubjectId = subject.SubjectId,
                                SubjectName = subject.SubjectName,
                                UnassignedPeriodsCount = remainingUnassigned,
                                Reason = $"Could not schedule {remainingUnassigned} period(s) due to faculty or room availability conflicts."
                            });
                            break;
                        }
                    }
                }
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var existingSectionDrafts = await _context.Timetables
                        .Where(t => dto.SectionIds.Contains(t.SectionId) && t.AcademicYearId == dto.AcademicYearId && t.ApprovalStatus == TimetableApprovalStatus.Draft && !t.IsPublished)
                        .ToListAsync();

                    if (existingSectionDrafts.Count > 0)
                    {
                        _context.Timetables.RemoveRange(existingSectionDrafts);
                    }

                    if (generatedDraftEntities.Count > 0)
                    {
                        await _context.Timetables.AddRangeAsync(generatedDraftEntities);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });

            var sectionTimetableDtos = new List<TimetableResponseDto>();
            foreach (var sec in targetSections)
            {
                var secSlots = await GetSectionTimetableAsync(sec.SectionId, dto.AcademicYearId, isPublished: false);
                sectionTimetableDtos.AddRange(secSlots);
            }

            return new GenerateTimetableResultDto
            {
                IsSuccess = true,
                Message = $"Successfully generated {generatedDraftEntities.Count} draft theory timetable slots across {targetSections.Count} section(s).",
                TotalSlotsGenerated = generatedDraftEntities.Count,
                SectionsProcessedCount = targetSections.Count,
                GeneratedSlots = sectionTimetableDtos,
                Warnings = warnings
            };
        }
    }
}
