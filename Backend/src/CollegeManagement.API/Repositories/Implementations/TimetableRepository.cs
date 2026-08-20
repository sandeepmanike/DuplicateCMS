using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Timetable;
using CollegeManagement.API.Models.Timetable;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly AppDbContext _context;

        public TimetableRepository(AppDbContext context)
        {
            _context = context;
        }

        private bool IsRelational => _context.Database.ProviderName != null && !_context.Database.ProviderName.Contains("InMemory");

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<TimetableResponseDto?> GetByIdAsync(int id)
        {
            if (IsRelational)
            {
                return await Connection.QueryFirstOrDefaultAsync<TimetableResponseDto>(
                    "sp_GetTimetableById",
                    new { p_Id = id },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var slots = await GetInMemoryTimetableDtosAsync(t => t.Id == id);
                return slots.FirstOrDefault();
            }
        }

        public async Task<(IEnumerable<TimetableResponseDto> Items, int TotalCount)> GetPagedAsync(TimetableQueryParams queryParams)
        {
            if (IsRelational)
            {
                var items = await Connection.QueryAsync<TimetableResponseDto>(
                    "sp_GetTimetables",
                    new
                    {
                        p_BoardId = queryParams.BoardId,
                        p_AcademicLevelId = queryParams.AcademicLevelId,
                        p_AcademicYearId = queryParams.AcademicYearId,
                        p_GroupId = queryParams.GroupId,
                        p_SectionId = queryParams.SectionId,
                        p_DayOfWeek = queryParams.DayOfWeek,
                        p_FacultyId = queryParams.FacultyId,
                        p_RoomId = queryParams.RoomId,
                        p_IsPublished = queryParams.IsPublished
                    },
                    commandType: CommandType.StoredProcedure);

                var list = items.ToList();
                int totalCount = list.Count;

                int skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
                var paged = list.Skip(skip).Take(queryParams.PageSize);

                return (paged, totalCount);
            }
            else
            {
                var list = await GetInMemoryTimetableDtosAsync(t =>
                    (queryParams.BoardId == null || t.BoardId == queryParams.BoardId) &&
                    (queryParams.AcademicLevelId == null || t.AcademicLevelId == queryParams.AcademicLevelId) &&
                    (queryParams.AcademicYearId == null || t.AcademicYearId == queryParams.AcademicYearId) &&
                    (queryParams.GroupId == null || t.GroupId == queryParams.GroupId) &&
                    (queryParams.SectionId == null || t.SectionId == queryParams.SectionId) &&
                    (queryParams.DayOfWeek == null || t.DayOfWeek == queryParams.DayOfWeek) &&
                    (queryParams.FacultyId == null || t.FacultyId == queryParams.FacultyId) &&
                    (queryParams.RoomId == null || t.RoomId == queryParams.RoomId) &&
                    (queryParams.IsPublished == null || t.IsPublished == queryParams.IsPublished));

                int totalCount = list.Count;
                int skip = (queryParams.PageNumber - 1) * queryParams.PageSize;
                var paged = list.Skip(skip).Take(queryParams.PageSize);

                return (paged, totalCount);
            }
        }

        public async Task<IEnumerable<TimetableResponseDto>> GetByFacultyIdAsync(int facultyId, int? academicYearId = null)
        {
            if (IsRelational)
            {
                return await Connection.QueryAsync<TimetableResponseDto>(
                    "sp_GetTimetables",
                    new
                    {
                        p_BoardId = (int?)null,
                        p_AcademicLevelId = (int?)null,
                        p_AcademicYearId = academicYearId,
                        p_GroupId = (int?)null,
                        p_SectionId = (int?)null,
                        p_DayOfWeek = (int?)null,
                        p_FacultyId = facultyId,
                        p_RoomId = (int?)null,
                        p_IsPublished = 1
                    },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                return await GetInMemoryTimetableDtosAsync(t => t.FacultyId == facultyId && (academicYearId == null || t.AcademicYearId == academicYearId) && t.IsPublished);
            }
        }

        public async Task<IEnumerable<TimetableResponseDto>> GetBySectionIdAsync(int sectionId, int? academicYearId = null, bool? isPublished = null)
        {
            if (IsRelational)
            {
                return await Connection.QueryAsync<TimetableResponseDto>(
                    "sp_GetTimetables",
                    new
                    {
                        p_BoardId = (int?)null,
                        p_AcademicLevelId = (int?)null,
                        p_AcademicYearId = academicYearId,
                        p_GroupId = (int?)null,
                        p_SectionId = sectionId,
                        p_DayOfWeek = (int?)null,
                        p_FacultyId = (int?)null,
                        p_RoomId = (int?)null,
                        p_IsPublished = isPublished
                    },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                return await GetInMemoryTimetableDtosAsync(t => t.SectionId == sectionId && (academicYearId == null || t.AcademicYearId == academicYearId) && (isPublished == null || t.IsPublished == isPublished));
            }
        }

        public async Task<int> AddAsync(CreateTimetableDto dto)
        {
            if (IsRelational)
            {
                return await Connection.ExecuteScalarAsync<int>(
                    "sp_CreateTimetable",
                    new
                    {
                        p_BoardId = dto.BoardId,
                        p_AcademicLevelId = dto.AcademicLevelId,
                        p_AcademicYearId = dto.AcademicYearId,
                        p_GroupId = dto.GroupId,
                        p_SectionId = dto.SectionId,
                        p_DayOfWeek = dto.DayOfWeek,
                        p_PeriodId = dto.PeriodId,
                        p_SubjectId = dto.SubjectId,
                        p_FacultyId = dto.FacultyId,
                        p_RoomId = dto.RoomId,
                        p_IsPublished = dto.IsPublished,
                        p_Remarks = dto.Remarks
                    },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var entity = new Timetable
                {
                    BoardId = dto.BoardId,
                    AcademicLevelId = dto.AcademicLevelId,
                    AcademicYearId = dto.AcademicYearId,
                    GroupId = dto.GroupId,
                    SectionId = dto.SectionId,
                    DayOfWeek = dto.DayOfWeek,
                    PeriodId = dto.PeriodId,
                    SubjectId = dto.SubjectId,
                    FacultyId = dto.FacultyId,
                    RoomId = dto.RoomId,
                    IsPublished = dto.IsPublished,
                    ApprovalStatus = dto.IsPublished ? TimetableApprovalStatus.Published : TimetableApprovalStatus.Draft,
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Timetables.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity.Id;
            }
        }

        public async Task UpdateAsync(int id, UpdateTimetableDto dto)
        {
            if (IsRelational)
            {
                await Connection.ExecuteAsync(
                    "sp_UpdateTimetable",
                    new
                    {
                        p_Id = id,
                        p_BoardId = dto.BoardId,
                        p_AcademicLevelId = dto.AcademicLevelId,
                        p_AcademicYearId = dto.AcademicYearId,
                        p_GroupId = dto.GroupId,
                        p_SectionId = dto.SectionId,
                        p_DayOfWeek = dto.DayOfWeek,
                        p_PeriodId = dto.PeriodId,
                        p_SubjectId = dto.SubjectId,
                        p_FacultyId = dto.FacultyId,
                        p_RoomId = dto.RoomId,
                        p_IsPublished = dto.IsPublished,
                        p_Remarks = dto.Remarks
                    },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var entity = await _context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
                if (entity != null)
                {
                    entity.BoardId = dto.BoardId;
                    entity.AcademicLevelId = dto.AcademicLevelId;
                    entity.AcademicYearId = dto.AcademicYearId;
                    entity.GroupId = dto.GroupId;
                    entity.SectionId = dto.SectionId;
                    entity.DayOfWeek = dto.DayOfWeek;
                    entity.PeriodId = dto.PeriodId;
                    entity.SubjectId = dto.SubjectId;
                    entity.FacultyId = dto.FacultyId;
                    entity.RoomId = dto.RoomId;
                    entity.IsPublished = dto.IsPublished;
                    entity.Remarks = dto.Remarks;
                    entity.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (IsRelational)
            {
                await Connection.ExecuteAsync(
                    "sp_DeleteTimetable",
                    new { p_Id = id },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var entity = await _context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
                if (entity != null)
                {
                    _context.Timetables.Remove(entity);
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task TogglePublishSlotAsync(int id, bool isPublished)
        {
            if (IsRelational)
            {
                await Connection.ExecuteAsync(
                    "sp_PublishTimetableSlot",
                    new { p_Id = id, p_IsPublished = isPublished },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var entity = await _context.Timetables.FirstOrDefaultAsync(t => t.Id == id);
                if (entity != null)
                {
                    entity.IsPublished = isPublished;
                    entity.ApprovalStatus = isPublished ? TimetableApprovalStatus.Published : TimetableApprovalStatus.Draft;
                    entity.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public async Task PublishSectionTimetableAsync(int sectionId, int academicYearId, bool isPublished)
        {
            if (IsRelational)
            {
                await Connection.ExecuteAsync(
                    "sp_PublishSectionTimetable",
                    new { p_SectionId = sectionId, p_AcademicYearId = academicYearId, p_IsPublished = isPublished },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var slots = await _context.Timetables.Where(t => t.SectionId == sectionId && t.AcademicYearId == academicYearId).ToListAsync();
                foreach (var s in slots)
                {
                    s.IsPublished = isPublished;
                    s.ApprovalStatus = isPublished ? TimetableApprovalStatus.Published : TimetableApprovalStatus.Draft;
                    s.UpdatedAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasSectionSlotConflictAsync(int academicYearId, int sectionId, int dayOfWeek, int periodId, int? excludeId = null)
        {
            if (IsRelational)
            {
                int count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckSectionSlotConflict",
                    new
                    {
                        p_AcademicYearId = academicYearId,
                        p_SectionId = sectionId,
                        p_DayOfWeek = dayOfWeek,
                        p_PeriodId = periodId,
                        p_ExcludeId = excludeId
                    },
                    commandType: CommandType.StoredProcedure);
                return count > 0;
            }
            else
            {
                return await _context.Timetables.AnyAsync(t =>
                    t.AcademicYearId == academicYearId &&
                    t.SectionId == sectionId &&
                    t.DayOfWeek == dayOfWeek &&
                    t.PeriodId == periodId &&
                    (excludeId == null || t.Id != excludeId.Value));
            }
        }

        public async Task<bool> HasFacultySlotConflictAsync(int academicYearId, int facultyId, int dayOfWeek, int periodId, int? excludeId = null)
        {
            if (IsRelational)
            {
                int count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckFacultySlotConflict",
                    new
                    {
                        p_AcademicYearId = academicYearId,
                        p_FacultyId = facultyId,
                        p_DayOfWeek = dayOfWeek,
                        p_PeriodId = periodId,
                        p_ExcludeId = excludeId
                    },
                    commandType: CommandType.StoredProcedure);
                return count > 0;
            }
            else
            {
                return await _context.Timetables.AnyAsync(t =>
                    t.AcademicYearId == academicYearId &&
                    t.FacultyId == facultyId &&
                    t.DayOfWeek == dayOfWeek &&
                    t.PeriodId == periodId &&
                    (excludeId == null || t.Id != excludeId.Value));
            }
        }

        public async Task<bool> HasRoomSlotConflictAsync(int academicYearId, int roomId, int dayOfWeek, int periodId, int? excludeId = null)
        {
            if (IsRelational)
            {
                int count = await Connection.ExecuteScalarAsync<int>(
                    "sp_CheckRoomSlotConflict",
                    new
                    {
                        p_AcademicYearId = academicYearId,
                        p_RoomId = roomId,
                        p_DayOfWeek = dayOfWeek,
                        p_PeriodId = periodId,
                        p_ExcludeId = excludeId
                    },
                    commandType: CommandType.StoredProcedure);
                return count > 0;
            }
            else
            {
                return await _context.Timetables.AnyAsync(t =>
                    t.AcademicYearId == academicYearId &&
                    t.RoomId == roomId &&
                    t.DayOfWeek == dayOfWeek &&
                    t.PeriodId == periodId &&
                    (excludeId == null || t.Id != excludeId.Value));
            }
        }

        public async Task<IEnumerable<AllocatedFacultyDto>> GetAllocatedFacultiesAsync(int? boardId, int? academicLevelId, int? academicYearId, int? groupId, int? sectionId, int? subjectId)
        {
            if (IsRelational)
            {
                return await Connection.QueryAsync<AllocatedFacultyDto>(
                    "sp_GetAllocatedFacultiesForSlot",
                    new
                    {
                        p_BoardId = boardId,
                        p_AcademicLevelId = academicLevelId,
                        p_AcademicYearId = academicYearId,
                        p_GroupId = groupId,
                        p_SectionId = sectionId,
                        p_SubjectId = subjectId
                    },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var allocations = await _context.FacultySubjectAllocations
                    .Include(a => a.Faculty)
                    .Where(a => (subjectId == null || a.SubjectId == subjectId) && a.Faculty != null && !a.Faculty.IsDeleted)
                    .ToListAsync();

                return allocations.Select(a => new AllocatedFacultyDto
                {
                    FacultyId = a.Faculty!.Id,
                    FacultyEmployeeId = a.Faculty.EmployeeId,
                    FacultyName = $"{a.Faculty.FirstName} {a.Faculty.LastName}",
                    Email = a.Faculty.Email ?? string.Empty,
                    Mobile = a.Faculty.Mobile ?? string.Empty,
                    Designation = a.Faculty.Designation ?? string.Empty
                });
            }
        }

        public async Task CopySectionTimetableAsync(CopyTimetableDto dto)
        {
            if (IsRelational)
            {
                await Connection.ExecuteAsync(
                    "sp_CopyTimetable",
                    new
                    {
                        p_SourceAcademicYearId = dto.SourceAcademicYearId,
                        p_SourceSectionId = dto.SourceSectionId,
                        p_TargetAcademicYearId = dto.TargetAcademicYearId,
                        p_TargetSectionId = dto.TargetSectionId
                    },
                    commandType: CommandType.StoredProcedure);
            }
            else
            {
                var sourceSlots = await _context.Timetables
                    .Where(t => t.SectionId == dto.SourceSectionId && t.AcademicYearId == dto.SourceAcademicYearId)
                    .ToListAsync();

                var targetSlots = sourceSlots.Select(s => new Timetable
                {
                    BoardId = s.BoardId,
                    AcademicLevelId = s.AcademicLevelId,
                    AcademicYearId = dto.TargetAcademicYearId,
                    GroupId = s.GroupId,
                    SectionId = dto.TargetSectionId,
                    DayOfWeek = s.DayOfWeek,
                    PeriodId = s.PeriodId,
                    SubjectId = s.SubjectId,
                    FacultyId = s.FacultyId,
                    RoomId = s.RoomId,
                    IsPublished = false,
                    ApprovalStatus = TimetableApprovalStatus.Draft,
                    Remarks = s.Remarks,
                    CreatedAt = DateTime.UtcNow
                }).ToList();

                await _context.Timetables.AddRangeAsync(targetSlots);
                await _context.SaveChangesAsync();
            }
        }

        private async Task<List<TimetableResponseDto>> GetInMemoryTimetableDtosAsync(System.Linq.Expressions.Expression<Func<Timetable, bool>> predicate)
        {
            var entities = await _context.Timetables.Where(predicate).ToListAsync();
            if (!entities.Any()) return new List<TimetableResponseDto>();

            var boards = await _context.Boards.ToDictionaryAsync(b => b.BoardId, b => b.BoardName);
            var levels = await _context.AcademicLevels.ToDictionaryAsync(l => l.AcademicLevelId, l => l.LevelName);
            var years = await _context.AcademicYears.ToDictionaryAsync(y => y.AcademicYearId, y => y.AcademicYearName);
            var groups = await _context.Groups.ToDictionaryAsync(g => g.GroupId, g => g.GroupName);
            var sections = await _context.Sections.ToDictionaryAsync(s => s.SectionId, s => s.SectionName);
            var periods = await _context.Periods.ToDictionaryAsync(p => p.PeriodId, p => p);
            var subjects = await _context.Subjects.ToDictionaryAsync(s => s.SubjectId, s => s);
            var faculties = await _context.Faculties.ToDictionaryAsync(f => f.Id, f => f);
            var rooms = await _context.Rooms.ToDictionaryAsync(r => r.RoomId, r => r);

            return entities.Select(e => new TimetableResponseDto
            {
                Id = e.Id,
                BoardId = e.BoardId,
                BoardName = boards.GetValueOrDefault(e.BoardId, string.Empty),
                AcademicLevelId = e.AcademicLevelId,
                LevelName = levels.GetValueOrDefault(e.AcademicLevelId, string.Empty),
                AcademicYearId = e.AcademicYearId,
                AcademicYearName = years.GetValueOrDefault(e.AcademicYearId, string.Empty),
                GroupId = e.GroupId,
                GroupName = groups.GetValueOrDefault(e.GroupId, string.Empty),
                SectionId = e.SectionId,
                SectionName = sections.GetValueOrDefault(e.SectionId, string.Empty),
                DayOfWeek = e.DayOfWeek,
                PeriodId = e.PeriodId,
                PeriodName = periods.TryGetValue(e.PeriodId, out var p) ? p.PeriodName : string.Empty,
                StartTime = periods.TryGetValue(e.PeriodId, out var pTime) ? pTime.StartTime : TimeSpan.Zero,
                EndTime = periods.TryGetValue(e.PeriodId, out var pEndTime) ? pEndTime.EndTime : TimeSpan.Zero,
                IsBreak = periods.TryGetValue(e.PeriodId, out var pBreak) && pBreak.IsBreak,
                SubjectId = e.SubjectId,
                SubjectCode = subjects.TryGetValue(e.SubjectId, out var sub) ? sub.SubjectCode : string.Empty,
                SubjectName = subjects.TryGetValue(e.SubjectId, out var subName) ? subName.SubjectName : string.Empty,
                FacultyId = e.FacultyId,
                FacultyEmployeeId = faculties.TryGetValue(e.FacultyId, out var fac) ? fac.EmployeeId : string.Empty,
                FacultyName = faculties.TryGetValue(e.FacultyId, out var facName) ? $"{facName.FirstName} {facName.LastName}" : string.Empty,
                RoomId = e.RoomId,
                RoomCode = rooms.TryGetValue(e.RoomId, out var rm) ? rm.RoomCode : string.Empty,
                RoomName = rooms.TryGetValue(e.RoomId, out var rmName) ? rmName.RoomName : string.Empty,
                IsPublished = e.IsPublished,
                ApprovalStatus = (int)e.ApprovalStatus,
                ApprovalStatusName = e.ApprovalStatus.ToString(),
                Remarks = e.Remarks,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }
    }
}
