using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Attendance.Requests;
using CollegeManagement.API.DTOs.Attendance.Responses;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace CollegeManagement.API.Repositories.Implementations
{
    /// <summary>
    /// Repository implementation for Attendance database operations using Dapper and Stored Procedures.
    /// </summary>
    public class AttendanceRepository : IAttendanceRepository
    {
        #region Stored Procedure Constants

        private const string SpCreateAttendance = "sp_CreateAttendance";
        private const string SpCreateBulkAttendance = "sp_CreateBulkAttendance";
        private const string SpUpdateAttendance = "sp_UpdateAttendance";
        private const string SpChangeAttendanceStatus = "sp_ChangeAttendanceStatus";
        private const string SpGetAttendanceById = "sp_GetAttendanceById";
        private const string SpGetAttendances = "sp_GetAttendances";
        private const string SpGetStudentsForAttendance = "sp_GetStudentsForAttendance";
        private const string SpGetAttendanceSummary = "sp_GetAttendanceSummary";
        private const string SpGetAttendancePercentage = "sp_GetAttendancePercentage";
        private const string SpGetAttendanceReport = "sp_GetAttendanceReport";
        private const string SpAttendanceExists = "sp_AttendanceExists";

        #endregion

        #region Constructor

        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttendanceRepository"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public AttendanceRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection => _context.Database.GetDbConnection();

        #endregion

        #region Helper Methods

        /// <summary>
        /// Builds common search dynamic parameters for attendance search requests.
        /// </summary>
        private DynamicParameters BuildSearchParameters(AttendanceSearchRequest request)
        {
            var parameters = new DynamicParameters();

            parameters.Add("p_BoardId", request.BoardId);
            parameters.Add("p_AcademicYearId", request.AcademicYearId);
            parameters.Add("p_AcademicLevelId", request.AcademicLevelId);
            parameters.Add("p_GroupId", request.GroupId);
            parameters.Add("p_SectionId", request.SectionId);
            parameters.Add("p_SubjectId", request.SubjectId);
            parameters.Add("p_FacultyId", request.FacultyId);
            parameters.Add("p_StudentId", request.StudentId);
            parameters.Add("p_Status", request.Status.HasValue ? (byte)request.Status.Value : (byte?)null);
            parameters.Add("p_FromDate", request.FromDate);
            parameters.Add("p_ToDate", request.ToDate);
            parameters.Add("p_PageNumber", request.PageNumber);
            parameters.Add("p_PageSize", request.PageSize);
            parameters.Add("p_SearchText", request.SearchText);

            return parameters;
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Creates a new attendance record in the database using stored procedure sp_CreateAttendance.
        /// </summary>
        public async Task<int> CreateAttendanceAsync(Attendance attendance)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_AttendanceDate", attendance.AttendanceDate);
            parameters.Add("p_StudentId", attendance.StudentId);
            parameters.Add("p_FacultyId", attendance.FacultyId);
            parameters.Add("p_BoardId", attendance.BoardId);
            parameters.Add("p_AcademicYearId", attendance.AcademicYearId);
            parameters.Add("p_AcademicLevelId", attendance.AcademicLevelId);
            parameters.Add("p_GroupId", attendance.GroupId);
            parameters.Add("p_SectionId", attendance.SectionId);
            parameters.Add("p_SubjectId", attendance.SubjectId);
            parameters.Add("p_Status", (byte)attendance.Status);
            parameters.Add("p_Remarks", attendance.Remarks);

            return await Connection.ExecuteScalarAsync<int>(
                SpCreateAttendance,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Creates multiple student attendance records in bulk using stored procedure sp_CreateBulkAttendance.
        /// </summary>
        public async Task<int> CreateBulkAttendanceAsync(IEnumerable<Attendance> attendances)
        {
            var json = JsonSerializer.Serialize(attendances);

            var parameters = new DynamicParameters();
            parameters.Add("p_AttendanceJson", json);

            return await Connection.ExecuteScalarAsync<int>(
                SpCreateBulkAttendance,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Updates an existing attendance record in the database using stored procedure sp_UpdateAttendance.
        /// </summary>
        public async Task<int> UpdateAttendanceAsync(Attendance attendance)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_AttendanceId", attendance.AttendanceId);
            parameters.Add("p_AttendanceDate", attendance.AttendanceDate);
            parameters.Add("p_StudentId", attendance.StudentId);
            parameters.Add("p_FacultyId", attendance.FacultyId);
            parameters.Add("p_BoardId", attendance.BoardId);
            parameters.Add("p_AcademicYearId", attendance.AcademicYearId);
            parameters.Add("p_AcademicLevelId", attendance.AcademicLevelId);
            parameters.Add("p_GroupId", attendance.GroupId);
            parameters.Add("p_SectionId", attendance.SectionId);
            parameters.Add("p_SubjectId", attendance.SubjectId);
            parameters.Add("p_Status", (byte)attendance.Status);
            parameters.Add("p_Remarks", attendance.Remarks);

            return await Connection.ExecuteAsync(
                SpUpdateAttendance,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Changes the active/inactive status of an attendance record using stored procedure sp_ChangeAttendanceStatus.
        /// </summary>
        public async Task<int> ChangeAttendanceActiveStatusAsync(int attendanceId, bool isActive)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_AttendanceId", attendanceId);
            parameters.Add("p_IsActive", isActive);

            return await Connection.ExecuteAsync(
                SpChangeAttendanceStatus,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        #endregion

        #region Queries

        /// <summary>
        /// Retrieves a single detailed attendance response by its unique identifier using stored procedure sp_GetAttendanceById.
        /// </summary>
        public async Task<AttendanceResponse?> GetAttendanceByIdAsync(int attendanceId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_AttendanceId", attendanceId);

            return await Connection.QueryFirstOrDefaultAsync<AttendanceResponse>(
                SpGetAttendanceById,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Retrieves a filtered list of attendance records using stored procedure sp_GetAttendances.
        /// </summary>
        public async Task<IEnumerable<AttendanceListResponse>> GetAttendancesAsync(AttendanceSearchRequest request)
        {
            var parameters = BuildSearchParameters(request);

            return await Connection.QueryAsync<AttendanceListResponse>(
                SpGetAttendances,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Retrieves students available to mark attendance for the specified criteria using stored procedure sp_GetStudentsForAttendance.
        /// </summary>
        public async Task<IEnumerable<StudentAttendanceResponse>> GetStudentsForAttendanceAsync(AttendanceSearchRequest request)
        {
            var parameters = BuildSearchParameters(request);

            return await Connection.QueryAsync<StudentAttendanceResponse>(
                SpGetStudentsForAttendance,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Retrieves statistical summary metrics for the specified filters using stored procedure sp_GetAttendanceSummary.
        /// </summary>
        public async Task<AttendanceSummaryResponse> GetAttendanceSummaryAsync(AttendanceSearchRequest request)
        {
            var parameters = BuildSearchParameters(request);

            var result = await Connection.QueryFirstOrDefaultAsync<AttendanceSummaryResponse>(
                SpGetAttendanceSummary,
                parameters,
                commandType: CommandType.StoredProcedure);

            return result ?? new AttendanceSummaryResponse();
        }

        /// <summary>
        /// Retrieves attendance percentages and class counts per student using stored procedure sp_GetAttendancePercentage.
        /// </summary>
        public async Task<IEnumerable<AttendancePercentageResponse>> GetAttendancePercentageAsync(AttendanceSearchRequest request)
        {
            var parameters = BuildSearchParameters(request);

            return await Connection.QueryAsync<AttendancePercentageResponse>(
                SpGetAttendancePercentage,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Generates a flat report listing attendance details for the specified filters using stored procedure sp_GetAttendanceReport.
        /// </summary>
        public async Task<IEnumerable<AttendanceReportResponse>> GetAttendanceReportAsync(AttendanceSearchRequest request)
        {
            var parameters = BuildSearchParameters(request);

            return await Connection.QueryAsync<AttendanceReportResponse>(
                SpGetAttendanceReport,
                parameters,
                commandType: CommandType.StoredProcedure);
        }

        /// <summary>
        /// Checks if an active attendance record already exists for a student, subject, and date using stored procedure sp_AttendanceExists.
        /// </summary>
        public async Task<bool> AttendanceExistsAsync(int studentId, int subjectId, DateTime attendanceDate)
        {
            var parameters = new DynamicParameters();
            parameters.Add("p_StudentId", studentId);
            parameters.Add("p_SubjectId", subjectId);
            parameters.Add("p_AttendanceDate", attendanceDate);

            var exists = await Connection.ExecuteScalarAsync<int>(
                SpAttendanceExists,
                parameters,
                commandType: CommandType.StoredProcedure);

            return exists > 0;
        }

        #endregion
    }
}
