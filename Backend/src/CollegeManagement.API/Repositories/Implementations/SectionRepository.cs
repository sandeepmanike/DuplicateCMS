using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Sections;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class SectionRepository : ISectionRepository
    {
        private readonly AppDbContext _context;

        public SectionRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        public async Task<IEnumerable<SectionResponse>> GetAllSectionsAsync(SectionFilterDto? filter = null)
        {
            var parameters = new DynamicParameters();
            
            // Resolve BoardId
            int? boardId = (filter?.BoardId.HasValue == true && filter.BoardId.Value > 0) 
                ? filter.BoardId.Value 
                : await ResolveBoardIdAsync(null, filter?.Board);
            parameters.Add("p_BoardId", boardId);

            parameters.Add("p_AcademicYearId", (filter?.AcademicYearId.HasValue == true && filter.AcademicYearId.Value > 0) ? filter.AcademicYearId.Value : null);

            // Resolve AcademicLevelId
            int? academicLevelId = (filter?.AcademicLevelId.HasValue == true && filter.AcademicLevelId.Value > 0)
                ? filter.AcademicLevelId.Value
                : await ResolveAcademicLevelIdAsync(null, filter?.AcademicLevel ?? filter?.YearOfStudy);
            parameters.Add("p_AcademicLevelId", academicLevelId);

            // Resolve GroupId
            int? groupId = (filter?.GroupId.HasValue == true && filter.GroupId.Value > 0)
                ? filter.GroupId.Value
                : await ResolveGroupIdAsync(null, filter?.Group);
            parameters.Add("p_GroupId", groupId);

            // Resolve GroupProgramId & ProgramId
            int? programId = (filter?.ProgramId.HasValue == true && filter.ProgramId.Value > 0)
                ? filter.ProgramId.Value
                : await ResolveProgramIdAsync(null, filter?.Programme ?? filter?.Program, groupId);

            int? groupProgramId = (filter?.GroupProgramId.HasValue == true && filter.GroupProgramId.Value > 0)
                ? filter.GroupProgramId.Value
                : await ResolveGroupProgramIdAsync(null, groupId, programId);

            parameters.Add("p_GroupProgramId", groupProgramId);
            parameters.Add("p_ProgramId", programId);

            parameters.Add("p_SearchTerm", string.IsNullOrWhiteSpace(filter?.SearchTerm ?? filter?.Search) ? null : (filter?.SearchTerm ?? filter?.Search)!.Trim());
            parameters.Add("p_IsActive", filter?.IsActive);

            var result = await Connection.QueryAsync<SectionResponse>(
                "sp_GetAllSections",
                parameters,
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<SectionResponse?> GetSectionByIdAsync(int id)
        {
            return await Connection.QueryFirstOrDefaultAsync<SectionResponse>(
                "sp_GetSectionById",
                new { p_SectionId = id },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateSectionAsync(Section section)
        {
            return await Connection.ExecuteScalarAsync<int>(
                "sp_CreateSection",
                new
                {
                    p_BoardId = section.BoardId,
                    p_AcademicYearId = section.AcademicYearId,
                    p_AcademicLevelId = section.AcademicLevelId,
                    p_GroupId = section.GroupId,
                    p_GroupProgramId = section.GroupProgramId,
                    p_ProgramId = section.ProgramId,
                    p_SectionName = section.SectionName,
                    p_RoomId = section.RoomId,
                    p_InchargeId = section.InchargeId,
                    p_MaximumStrength = section.MaximumStrength,
                    p_IsActive = section.IsActive
                },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<bool> UpdateSectionAsync(int id, Section section)
        {
            var affected = await Connection.ExecuteAsync(
                "sp_UpdateSection",
                new
                {
                    p_SectionId = id,
                    p_BoardId = section.BoardId,
                    p_AcademicYearId = section.AcademicYearId,
                    p_AcademicLevelId = section.AcademicLevelId,
                    p_GroupId = section.GroupId,
                    p_GroupProgramId = section.GroupProgramId,
                    p_ProgramId = section.ProgramId,
                    p_SectionName = section.SectionName,
                    p_RoomId = section.RoomId,
                    p_InchargeId = section.InchargeId,
                    p_MaximumStrength = section.MaximumStrength,
                    p_IsActive = section.IsActive
                },
                commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        public async Task<bool> DeleteSectionAsync(int id)
        {
            var affected = await Connection.ExecuteAsync(
                "sp_DeleteSection",
                new { p_SectionId = id },
                commandType: CommandType.StoredProcedure);
            return affected > 0;
        }

        public async Task<IEnumerable<SectionResponse>> GetSectionsByGroupAsync(int groupId)
        {
            var result = await Connection.QueryAsync<SectionResponse>(
                "sp_GetSectionsByGroupId",
                new { p_GroupId = groupId },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<IEnumerable<SectionResponse>> GetSectionsByGroupProgramAsync(int groupProgramId)
        {
            var result = await Connection.QueryAsync<SectionResponse>(
                "sp_GetSectionsByGroupProgramId",
                new { p_GroupProgramId = groupProgramId },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<bool> IsSectionNameDuplicateAsync(
            int? boardId,
            int academicYearId,
            int? academicLevelId,
            int? groupId,
            int? groupProgramId,
            int? programId,
            string sectionName,
            int? excludeSectionId = null)
        {
            var sql = @"
                SELECT COUNT(1) FROM `Sections`
                WHERE AcademicYearId = @AcademicYearId
                  AND LOWER(TRIM(SectionName)) = LOWER(TRIM(@SectionName))
                  AND (@BoardId IS NULL OR BoardId = @BoardId)
                  AND (@AcademicLevelId IS NULL OR AcademicLevelId = @AcademicLevelId)
                  AND (@GroupId IS NULL OR GroupId = @GroupId)
                  AND (@GroupProgramId IS NULL OR GroupProgramId = @GroupProgramId)
                  AND (@ProgramId IS NULL OR ProgramId = @ProgramId)
                  AND (@ExcludeSectionId IS NULL OR SectionId <> @ExcludeSectionId);";

            var count = await Connection.ExecuteScalarAsync<int>(sql, new
            {
                AcademicYearId = academicYearId,
                SectionName = sectionName,
                BoardId = boardId,
                AcademicLevelId = academicLevelId,
                GroupId = groupId,
                GroupProgramId = groupProgramId,
                ProgramId = programId,
                ExcludeSectionId = excludeSectionId
            });

            return count > 0;
        }

        public async Task<bool> AcademicYearExistsAsync(int academicYearId)
        {
            var count = await Connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM AcademicYears WHERE AcademicYearId = @Id",
                new { Id = academicYearId });
            return count > 0;
        }

        public async Task<AcademicYear?> GetAcademicYearByIdAsync(int academicYearId)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT AcademicYearId, AcademicYearName, StartDate, EndDate, AdmissionStartDate, AdmissionEndDate, IsActive FROM AcademicYears WHERE AcademicYearId = @Id",
                new { Id = academicYearId });

            if (row == null) return null;

            DateOnly ToDateOnly(dynamic? val)
            {
                if (val == null) return default;
                if (val is DateOnly d) return d;
                if (val is DateTime dt) return DateOnly.FromDateTime(dt);
                if (DateTime.TryParse(val.ToString(), out DateTime parsed)) return DateOnly.FromDateTime(parsed);
                return default;
            }

            DateOnly? ToNullableDateOnly(dynamic? val)
            {
                if (val == null) return null;
                if (val is DateOnly d) return d;
                if (val is DateTime dt) return DateOnly.FromDateTime(dt);
                if (DateTime.TryParse(val.ToString(), out DateTime parsed)) return DateOnly.FromDateTime(parsed);
                return null;
            }

            return new AcademicYear
            {
                AcademicYearId = (int)row.AcademicYearId,
                AcademicYearName = row.AcademicYearName?.ToString() ?? string.Empty,
                StartDate = ToDateOnly(row.StartDate),
                EndDate = ToDateOnly(row.EndDate),
                AdmissionStartDate = ToNullableDateOnly(row.AdmissionStartDate),
                AdmissionEndDate = ToNullableDateOnly(row.AdmissionEndDate),
                IsActive = row.IsActive is bool b ? b : (row.IsActive is int i ? i == 1 : Convert.ToBoolean(row.IsActive))
            };
        }

        public async Task<bool> FacultyExistsAsync(int facultyId)
        {
            try
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Staffs WHERE Id = @Id AND (IsDeleted = 0 OR IsDeleted IS NULL)",
                    new { Id = facultyId });
                return count > 0;
            }
            catch
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Faculties WHERE Id = @Id AND (IsDeleted = 0 OR IsDeleted IS NULL)",
                    new { Id = facultyId });
                return count > 0;
            }
        }

        public async Task<bool> RoomExistsAsync(int roomId)
        {
            var count = await Connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Rooms WHERE RoomId = @Id AND IsActive = 1",
                new { Id = roomId });
            return count > 0;
        }

        public async Task<CollegeManagement.API.Models.Timetable.Room?> GetRoomDetailsAsync(int? roomId, string? roomCode)
        {
            if (roomId.HasValue && roomId.Value > 0)
            {
                return await Connection.QueryFirstOrDefaultAsync<CollegeManagement.API.Models.Timetable.Room>(
                    "SELECT RoomId, RoomNumber, RoomCode, RoomName, BlockName, BlockName AS BuildingName, Floor, Capacity, RoomType, IsActive FROM Rooms WHERE RoomId = @Id",
                    new { Id = roomId.Value });
            }

            if (!string.IsNullOrWhiteSpace(roomCode))
            {
                var trimmed = roomCode.Trim();
                return await Connection.QueryFirstOrDefaultAsync<CollegeManagement.API.Models.Timetable.Room>(
                    "SELECT RoomId, RoomNumber, RoomCode, RoomName, BlockName, BlockName AS BuildingName, Floor, Capacity, RoomType, IsActive FROM Rooms WHERE RoomCode = @Code OR RoomNumber = @Code LIMIT 1",
                    new { Code = trimmed });
            }

            return null;
        }

        public async Task<SectionResponse?> GetActiveSectionAssignedToRoomAsync(int? roomId, string? roomCode, int? excludeSectionId = null)
        {
            var sql = @"
                SELECT SectionId, SectionName, RoomId, IsActive
                FROM `Sections`
                WHERE IsActive = 1
                  AND (
                      (@RoomId IS NOT NULL AND @RoomId > 0 AND RoomId = @RoomId)
                      OR (@RoomCode IS NOT NULL AND @RoomCode <> '' AND RoomId IN (SELECT RoomId FROM Rooms WHERE RoomCode = @RoomCode OR RoomNumber = @RoomCode))
                  )
                  AND (@ExcludeSectionId IS NULL OR SectionId <> @ExcludeSectionId)
                LIMIT 1;";

            return await Connection.QueryFirstOrDefaultAsync<SectionResponse>(
                sql,
                new
                {
                    RoomId = roomId,
                    RoomCode = string.IsNullOrWhiteSpace(roomCode) ? null : roomCode.Trim(),
                    ExcludeSectionId = excludeSectionId
                });
        }

        public async Task<int?> ResolveBoardIdAsync(int? boardId, string? boardName)
        {
            if (boardId.HasValue && boardId.Value > 0) return boardId.Value;
            if (string.IsNullOrWhiteSpace(boardName)) return null;

            var trimmed = boardName.Trim();
            return await Connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT BoardId FROM Boards WHERE LOWER(TRIM(BoardName)) = LOWER(TRIM(@Name)) OR LOWER(TRIM(BoardCode)) = LOWER(TRIM(@Name)) LIMIT 1;",
                new { Name = trimmed });
        }

        public async Task<int?> ResolveGroupIdAsync(int? groupId, string? groupName)
        {
            if (groupId.HasValue && groupId.Value > 0) return groupId.Value;
            if (string.IsNullOrWhiteSpace(groupName)) return null;

            var trimmed = groupName.Trim();
            return await Connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT GroupId FROM `Groups` WHERE LOWER(TRIM(GroupName)) = LOWER(TRIM(@Name)) OR LOWER(TRIM(GroupCode)) = LOWER(TRIM(@Name)) LIMIT 1;",
                new { Name = trimmed });
        }

        public async Task<int?> ResolveAcademicLevelIdAsync(int? academicLevelId, string? levelName)
        {
            if (academicLevelId.HasValue && academicLevelId.Value > 0) return academicLevelId.Value;
            if (string.IsNullOrWhiteSpace(levelName)) return null;

            var trimmed = levelName.Trim();
            return await Connection.QueryFirstOrDefaultAsync<int?>(
                "SELECT AcademicLevelId FROM AcademicLevels WHERE LOWER(TRIM(LevelName)) = LOWER(TRIM(@Name)) OR LOWER(TRIM(LevelCode)) = LOWER(TRIM(@Name)) LIMIT 1;",
                new { Name = trimmed });
        }

        public async Task<int?> ResolveProgramIdAsync(int? programId, string? programName, int? groupId)
        {
            if (programId.HasValue && programId.Value > 0) return programId.Value;

            if (!string.IsNullOrWhiteSpace(programName))
            {
                var trimmed = programName.Trim();
                var id = await Connection.QueryFirstOrDefaultAsync<int?>(
                    "SELECT ProgramId FROM `Programs` WHERE LOWER(TRIM(ProgramName)) = LOWER(TRIM(@Name)) LIMIT 1;",
                    new { Name = trimmed });
                if (id.HasValue && id.Value > 0) return id.Value;
            }

            if (groupId.HasValue && groupId.Value > 0)
            {
                return await Connection.QueryFirstOrDefaultAsync<int?>(
                    "SELECT ProgramId FROM `GroupPrograms` WHERE GroupId = @GroupId AND IsActive = 1 ORDER BY GroupProgramId ASC LIMIT 1;",
                    new { GroupId = groupId.Value });
            }

            return null;
        }

        public async Task<int?> ResolveGroupProgramIdAsync(int? groupProgramId, int? groupId, int? programId)
        {
            if (groupProgramId.HasValue && groupProgramId.Value > 0) return groupProgramId.Value;

            if (groupId.HasValue && groupId.Value > 0 && programId.HasValue && programId.Value > 0)
            {
                var id = await Connection.QueryFirstOrDefaultAsync<int?>(
                    "SELECT GroupProgramId FROM `GroupPrograms` WHERE GroupId = @GroupId AND ProgramId = @ProgramId AND IsActive = 1 LIMIT 1;",
                    new { GroupId = groupId.Value, ProgramId = programId.Value });
                if (id.HasValue && id.Value > 0) return id.Value;
            }

            if (groupId.HasValue && groupId.Value > 0)
            {
                return await Connection.QueryFirstOrDefaultAsync<int?>(
                    "SELECT GroupProgramId FROM `GroupPrograms` WHERE GroupId = @GroupId AND IsActive = 1 ORDER BY GroupProgramId ASC LIMIT 1;",
                    new { GroupId = groupId.Value });
            }

            return null;
        }

        public async Task<(int? GroupId, int? ProgramId)> GetGroupAndProgramByGroupProgramIdAsync(int groupProgramId)
        {
            var row = await Connection.QueryFirstOrDefaultAsync<dynamic>(
                "SELECT GroupId, ProgramId FROM `GroupPrograms` WHERE GroupProgramId = @Id LIMIT 1;",
                new { Id = groupProgramId });

            if (row == null) return (null, null);
            return ((int?)row.GroupId, (int?)row.ProgramId);
        }

        public async Task<bool> IsProgramValidForGroupAsync(int groupId, int programId)
        {
            var count = await Connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM `GroupPrograms` WHERE GroupId = @GroupId AND ProgramId = @ProgramId AND IsActive = 1;",
                new { GroupId = groupId, ProgramId = programId });
            return count > 0;
        }
    }
}
