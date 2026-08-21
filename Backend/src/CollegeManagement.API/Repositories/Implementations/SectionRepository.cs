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
            parameters.Add("p_Board", string.IsNullOrWhiteSpace(filter?.Board) ? null : filter.Board.Trim());
            parameters.Add("p_AcademicYearId", (filter?.AcademicYearId.HasValue == true && filter.AcademicYearId.Value > 0) ? filter.AcademicYearId.Value : null);
            parameters.Add("p_Group", string.IsNullOrWhiteSpace(filter?.Group) ? null : filter.Group.Trim());
            parameters.Add("p_GroupId", (filter?.GroupId.HasValue == true && filter.GroupId.Value > 0) ? filter.GroupId.Value : null);
            parameters.Add("p_Programme", string.IsNullOrWhiteSpace(filter?.Programme) ? null : filter.Programme.Trim());
            parameters.Add("p_AcademicLevel", string.IsNullOrWhiteSpace(filter?.AcademicLevel) ? null : filter.AcademicLevel.Trim());
            parameters.Add("p_SearchTerm", string.IsNullOrWhiteSpace(filter?.SearchTerm) ? null : filter.SearchTerm.Trim());
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
                    p_Board = section.Board,
                    p_BoardId = section.BoardId,
                    p_AcademicYearId = section.AcademicYearId,
                    p_Group = section.Group,
                    p_GroupId = section.GroupId,
                    p_Programme = section.Programme ?? string.Empty,
                    p_AcademicLevel = section.AcademicLevel,
                    p_SectionName = section.SectionName,
                    p_RoomNumber = section.RoomNumber,
                    p_InchargeId = section.InchargeId ?? section.ClassTeacherId,
                    p_MaximumStrength = section.MaximumStrength,
                    p_IsActive = section.IsActive,
                    p_RoomId = section.RoomId
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
                    p_Board = section.Board,
                    p_BoardId = section.BoardId,
                    p_AcademicYearId = section.AcademicYearId,
                    p_Group = section.Group,
                    p_GroupId = section.GroupId,
                    p_Programme = section.Programme ?? string.Empty,
                    p_AcademicLevel = section.AcademicLevel,
                    p_SectionName = section.SectionName,
                    p_RoomNumber = section.RoomNumber,
                    p_InchargeId = section.InchargeId ?? section.ClassTeacherId,
                    p_MaximumStrength = section.MaximumStrength,
                    p_IsActive = section.IsActive,
                    p_RoomId = section.RoomId
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
                "sp_GetSectionsByGroup",
                new { p_GroupId = groupId },
                commandType: CommandType.StoredProcedure);
            return result;
        }

        public async Task<bool> IsSectionNameDuplicateAsync(string board, int academicYearId, string group, string programme, string academicLevel, string sectionName, int? excludeSectionId = null)
        {
            var count = await Connection.ExecuteScalarAsync<int>(
                "sp_ValidateSectionName",
                new
                {
                    p_Board = board,
                    p_AcademicYearId = academicYearId,
                    p_Group = group,
                    p_Programme = programme ?? string.Empty,
                    p_AcademicLevel = academicLevel,
                    p_SectionName = sectionName,
                    p_ExcludeSectionId = excludeSectionId
                },
                commandType: CommandType.StoredProcedure);
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
                    "SELECT COUNT(1) FROM Faculties WHERE Id = @Id AND (IsDeleted = 0 OR IsDeleted IS NULL)",
                    new { Id = facultyId });
                return count > 0;
            }
            catch
            {
                var count = await Connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Faculty WHERE Id = @Id AND (IsDeleted = 0 OR IsDeleted IS NULL)",
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
                SELECT SectionId, SectionName, RoomNumber, RoomId, IsActive
                FROM Sections
                WHERE IsActive = 1
                  AND (
                      (@RoomId IS NOT NULL AND @RoomId > 0 AND RoomId = @RoomId)
                      OR (@RoomCode IS NOT NULL AND @RoomCode <> '' AND (RoomNumber = @RoomCode OR RoomId IN (SELECT RoomId FROM Rooms WHERE RoomCode = @RoomCode OR RoomNumber = @RoomCode)))
                  )
                  AND (@ExcludeSectionId IS NULL OR SectionId <> @ExcludeSectionId)
                LIMIT 1";

            return await Connection.QueryFirstOrDefaultAsync<SectionResponse>(
                sql,
                new
                {
                    RoomId = roomId,
                    RoomCode = string.IsNullOrWhiteSpace(roomCode) ? null : roomCode.Trim(),
                    ExcludeSectionId = excludeSectionId
                });
        }
    }
}
