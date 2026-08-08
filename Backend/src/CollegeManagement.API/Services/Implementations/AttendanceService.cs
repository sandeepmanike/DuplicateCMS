using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.DTOs.Attendance.Requests;
using CollegeManagement.API.DTOs.Attendance.Responses;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;

namespace CollegeManagement.API.Services.Implementations
{
    /// <summary>
    /// Service implementation for managing Attendance operations and business validations.
    /// </summary>
    public class AttendanceService : IAttendanceService
    {
        #region Constructor

        private readonly IAttendanceRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttendanceService"/> class.
        /// </summary>
        /// <param name="repository">The attendance repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        public AttendanceService(IAttendanceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Creates a new attendance record after performing duplicate validation checks.
        /// </summary>
        public async Task<int> CreateAttendanceAsync(CreateAttendanceRequest request)
        {
            var exists = await _repository.AttendanceExistsAsync(request.StudentId, request.SubjectId, request.AttendanceDate);
            if (exists)
            {
                throw new ConflictException($"Attendance has already been marked for this student on {request.AttendanceDate:yyyy-MM-dd}.");
            }

            var attendance = _mapper.Map<Attendance>(request);
            return await _repository.CreateAttendanceAsync(attendance);
        }

        /// <summary>
        /// Creates student attendance records in bulk after ensuring no duplicate attendance exists for any student in the list.
        /// </summary>
        public async Task<int> CreateBulkAttendanceAsync(BulkAttendanceRequest request)
        {
            if (request.Students == null || !request.Students.Any())
            {
                throw new ValidationException("Students list cannot be null or empty.");
            }

            var attendances = new List<Attendance>();

            foreach (var studentRequest in request.Students)
            {
                var exists = await _repository.AttendanceExistsAsync(studentRequest.StudentId, request.SubjectId, request.AttendanceDate);
                if (exists)
                {
                    throw new ConflictException($"Attendance record already exists for Student ID {studentRequest.StudentId}, Subject ID {request.SubjectId} on {request.AttendanceDate:yyyy-MM-dd}.");
                }

                var attendance = _mapper.Map<Attendance>(studentRequest);

                // Populate common bulk header values
                attendance.AttendanceDate = request.AttendanceDate;
                attendance.BoardId = request.BoardId;
                attendance.AcademicYearId = request.AcademicYearId;
                attendance.AcademicLevelId = request.AcademicLevelId;
                attendance.GroupId = request.GroupId;
                attendance.SectionId = request.SectionId;
                attendance.SubjectId = request.SubjectId;
                attendance.FacultyId = request.FacultyId;

                attendances.Add(attendance);
            }

            return await _repository.CreateBulkAttendanceAsync(attendances);
        }

        /// <summary>
        /// Updates an existing attendance record after validating its existence.
        /// </summary>
        public async Task<int> UpdateAttendanceAsync(UpdateAttendanceRequest request)
        {
            var existing = await _repository.GetAttendanceByIdAsync(request.AttendanceId);
            if (existing == null)
            {
                throw new NotFoundException($"Attendance record with ID {request.AttendanceId} was not found.");
            }

            var attendance = _mapper.Map<Attendance>(request);
            return await _repository.UpdateAttendanceAsync(attendance);
        }

        /// <summary>
        /// Validates existence and updates the active status of an attendance record.
        /// </summary>
        public async Task<int> ChangeAttendanceActiveStatusAsync(int attendanceId, bool isActive)
        {
            var existing = await _repository.GetAttendanceByIdAsync(attendanceId);
            if (existing == null)
            {
                throw new NotFoundException($"Attendance record with ID {attendanceId} was not found.");
            }

            return await _repository.ChangeAttendanceActiveStatusAsync(attendanceId, isActive);
        }

        #endregion

        #region Queries

        /// <summary>
        /// Retrieves a single detailed attendance record by its ID, throwing an exception if not found.
        /// </summary>
        public async Task<AttendanceResponse?> GetAttendanceByIdAsync(int attendanceId)
        {
            var result = await _repository.GetAttendanceByIdAsync(attendanceId);
            if (result == null)
            {
                throw new NotFoundException($"Attendance record with ID {attendanceId} was not found.");
            }
            return result;
        }

        /// <summary>
        /// Retrieves a filtered list of attendance records.
        /// </summary>
        public async Task<IEnumerable<AttendanceListResponse>> GetAttendancesAsync(AttendanceSearchRequest request)
        {
            return await _repository.GetAttendancesAsync(request);
        }

        /// <summary>
        /// Retrieves students available to mark attendance for the specified criteria.
        /// </summary>
        public async Task<IEnumerable<StudentAttendanceResponse>> GetStudentsForAttendanceAsync(AttendanceSearchRequest request)
        {
            return await _repository.GetStudentsForAttendanceAsync(request);
        }

        /// <summary>
        /// Retrieves statistical summary metrics for the specified filters.
        /// </summary>
        public async Task<AttendanceSummaryResponse> GetAttendanceSummaryAsync(AttendanceSearchRequest request)
        {
            return await _repository.GetAttendanceSummaryAsync(request);
        }

        /// <summary>
        /// Retrieves attendance percentages and class counts per student for the specified filters.
        /// </summary>
        public async Task<IEnumerable<AttendancePercentageResponse>> GetAttendancePercentageAsync(AttendanceSearchRequest request)
        {
            return await _repository.GetAttendancePercentageAsync(request);
        }

        /// <summary>
        /// Generates a flat report listing attendance details for the specified filters.
        /// </summary>
        public async Task<IEnumerable<AttendanceReportResponse>> GetAttendanceReportAsync(AttendanceSearchRequest request)
        {
            return await _repository.GetAttendanceReportAsync(request);
        }

        #endregion
    }
}
