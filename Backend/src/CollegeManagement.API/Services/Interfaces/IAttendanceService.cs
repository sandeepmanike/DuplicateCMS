using System.Collections.Generic;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Attendance.Requests;
using CollegeManagement.API.DTOs.Attendance.Responses;

namespace CollegeManagement.API.Services.Interfaces
{
    /// <summary>
    /// Service interface for Attendance operations.
    /// </summary>
    public interface IAttendanceService
    {
        /// <summary>
        /// Creates a new attendance record after performing business validations.
        /// </summary>
        /// <param name="request">The creation details.</param>
        /// <returns>The ID of the newly created attendance record.</returns>
        Task<int> CreateAttendanceAsync(CreateAttendanceRequest request);

        /// <summary>
        /// Creates student attendance records in bulk after performing business validations.
        /// </summary>
        /// <param name="request">The bulk attendance details.</param>
        /// <returns>The number of records successfully created.</returns>
        Task<int> CreateBulkAttendanceAsync(BulkAttendanceRequest request);

        /// <summary>
        /// Updates an existing attendance record after performing business validations.
        /// </summary>
        /// <param name="request">The update details.</param>
        /// <returns>The number of affected records.</returns>
        Task<int> UpdateAttendanceAsync(UpdateAttendanceRequest request);

        /// <summary>
        /// Retrieves a single detailed attendance record by its ID.
        /// </summary>
        /// <param name="attendanceId">The attendance identifier.</param>
        /// <returns>The matching attendance response DTO, or null if not found.</returns>
        Task<AttendanceResponse?> GetAttendanceByIdAsync(int attendanceId);

        /// <summary>
        /// Retrieves a filtered list of attendance records.
        /// </summary>
        /// <param name="request">The search and filter parameters.</param>
        /// <returns>A collection of attendance list response DTOs.</returns>
        Task<IEnumerable<AttendanceListResponse>> GetAttendancesAsync(AttendanceSearchRequest request);

        /// <summary>
        /// Retrieves students available to mark attendance for the specified search criteria.
        /// </summary>
        /// <param name="request">The search and filter parameters.</param>
        /// <returns>A collection of student attendance response DTOs.</returns>
        Task<IEnumerable<StudentAttendanceResponse>> GetStudentsForAttendanceAsync(AttendanceSearchRequest request);

        /// <summary>
        /// Retrieves statistical summary metrics (present, absent, late, leave counts) for the specified filters.
        /// </summary>
        /// <param name="request">The search and filter parameters.</param>
        /// <returns>An attendance summary response DTO.</returns>
        Task<AttendanceSummaryResponse> GetAttendanceSummaryAsync(AttendanceSearchRequest request);

        /// <summary>
        /// Retrieves attendance percentages and class counts per student for the specified filters.
        /// </summary>
        /// <param name="request">The search and filter parameters.</param>
        /// <returns>A collection of student attendance percentage DTOs.</returns>
        Task<IEnumerable<AttendancePercentageResponse>> GetAttendancePercentageAsync(AttendanceSearchRequest request);

        /// <summary>
        /// Generates a flat report listing attendance details for the specified filters.
        /// </summary>
        /// <param name="request">The search and filter parameters.</param>
        /// <returns>A collection of attendance report DTOs.</returns>
        Task<IEnumerable<AttendanceReportResponse>> GetAttendanceReportAsync(AttendanceSearchRequest request);

        /// <summary>
        /// Changes the active/inactive status of an attendance record.
        /// </summary>
        /// <param name="attendanceId">The attendance identifier.</param>
        /// <param name="isActive">The new active status flag.</param>
        /// <returns>The number of affected records.</returns>
        Task<int> ChangeAttendanceActiveStatusAsync(int attendanceId, bool isActive);
    }
}
