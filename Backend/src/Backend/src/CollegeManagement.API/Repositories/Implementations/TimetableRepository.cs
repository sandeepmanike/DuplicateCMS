using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Timetable;
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

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<TimetableResponseDto?> GetByIdAsync(int id)
        {
            return await Connection.QueryFirstOrDefaultAsync<TimetableResponseDto>(
                "sp_GetTimetableById",
                new { p_Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<(IEnumerable<TimetableResponseDto> Items, int TotalCount)> GetPagedAsync(TimetableQueryParams queryParams)
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

        public async Task<IEnumerable<TimetableResponseDto>> GetByFacultyIdAsync(int facultyId, int? academicYearId = null)
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

        public async Task<IEnumerable<TimetableResponseDto>> GetBySectionIdAsync(int sectionId, int? academicYearId = null, bool? isPublished = null)
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

        public async Task<int> AddAsync(CreateTimetableDto dto)
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

        public async Task UpdateAsync(int id, UpdateTimetableDto dto)
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

        public async Task DeleteAsync(int id)
        {
            await Connection.ExecuteAsync(
                "sp_DeleteTimetable",
                new { p_Id = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task TogglePublishSlotAsync(int id, bool isPublished)
        {
            await Connection.ExecuteAsync(
                "sp_PublishTimetableSlot",
                new { p_Id = id, p_IsPublished = isPublished },
                commandType: CommandType.StoredProcedure);
        }

        public async Task PublishSectionTimetableAsync(int sectionId, int academicYearId, bool isPublished)
        {
            await Connection.ExecuteAsync(
                "sp_PublishSectionTimetable",
                new { p_SectionId = sectionId, p_AcademicYearId = academicYearId, p_IsPublished = isPublished },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> HasSectionSlotConflictAsync(int academicYearId, int sectionId, int dayOfWeek, int periodId, int? excludeId = null)
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

        public async Task<bool> HasFacultySlotConflictAsync(int academicYearId, int facultyId, int dayOfWeek, int periodId, int? excludeId = null)
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

        public async Task<bool> HasRoomSlotConflictAsync(int academicYearId, int roomId, int dayOfWeek, int periodId, int? excludeId = null)
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

        public async Task<IEnumerable<AllocatedFacultyDto>> GetAllocatedFacultiesAsync(int? boardId, int? academicLevelId, int? academicYearId, int? groupId, int? sectionId, int? subjectId)
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

        public async Task CopySectionTimetableAsync(CopyTimetableDto dto)
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
    }
}
