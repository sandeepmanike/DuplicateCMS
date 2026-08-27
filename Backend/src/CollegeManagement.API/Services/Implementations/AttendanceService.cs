using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Attendance.Requests;
using CollegeManagement.API.DTOs.Attendance.Responses;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.DTOs.Common;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using CollegeManagement.API.Models.Reports;
using System.IO;
using System.Text;
using MiniExcelLibs;

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
        private readonly AppDbContext _context;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly IAttendanceCacheService _attendanceCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttendanceService"/> class.
        /// </summary>
        /// <param name="repository">The attendance repository.</param>
        /// <param name="mapper">The AutoMapper instance.</param>
        /// <param name="context">The database context.</param>
        /// <param name="auditLogRepository">The audit log repository.</param>
        /// <param name="attendanceCache">The attendance cache service.</param>
        public AttendanceService(
            IAttendanceRepository repository,
            IMapper mapper,
            AppDbContext context,
            IAuditLogRepository auditLogRepository,
            IAttendanceCacheService attendanceCache)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
            _auditLogRepository = auditLogRepository;
            _attendanceCache = attendanceCache;
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Creates a new attendance record after performing duplicate validation checks and master data verification.
        /// </summary>
        public async Task<int> CreateAttendanceAsync(CreateAttendanceRequest request, bool isAdmin, string userName)
        {
            if (request == null)
            {
                throw new ValidationException("Request body cannot be null.");
            }

            // 1. Validate student exists, is active, and belongs to Section
            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
            {
                throw new NotFoundException($"Student with ID {request.StudentId} was not found.");
            }
            if (!student.IsActive)
            {
                throw new ValidationException($"Student with ID {request.StudentId} is not active.");
            }
            if (student.SectionId != request.SectionId)
            {
                throw new ValidationException($"Student with ID {request.StudentId} does not belong to Section ID {request.SectionId}.");
            }

            // 2. Validate other master IDs
            await ValidateMasterDataAsync(request.FacultyId, request.BoardId, request.AcademicYearId, request.AcademicLevelId, request.GroupId, request.SectionId, request.SubjectId);

            // 3. Timetable/Period Checks
            if (request.TimetableId.HasValue)
            {
                var timetable = await _context.Timetables.FindAsync(request.TimetableId.Value);
                if (timetable == null || !timetable.IsPublished)
                {
                    throw new ValidationException($"Published Timetable slot with ID {request.TimetableId.Value} was not found.");
                }
                if (timetable.PeriodId != request.PeriodId)
                {
                    throw new ValidationException($"Timetable slot Period ID {timetable.PeriodId} does not match request Period ID {request.PeriodId}.");
                }
                if (timetable.SectionId != request.SectionId || timetable.SubjectId != request.SubjectId || timetable.StaffId != request.FacultyId)
                {
                    throw new ValidationException("Timetable structural elements (Section, Subject, Faculty) do not match request parameters.");
                }
            }
            else
            {
                // Ad-hoc: Validate FacultySubjectAllocation is active
                var subjectName = await _context.Subjects.Where(s => s.SubjectId == request.SubjectId).Select(s => s.SubjectName).FirstOrDefaultAsync();
                var allocated = await _context.FacultySubjectAllocations.AnyAsync(a => a.FacultyId == request.FacultyId && a.SubjectId == request.SubjectId);
                if (!allocated)
                {
                    throw new ValidationException($"Faculty member {request.FacultyId} is not allocated to subject {request.SubjectId}.");
                }
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 4. Resolve or Create AttendanceSession
                    var session = await GetOrCreateSessionAsync(request.SectionId, request.PeriodId, request.AttendanceDate, request);

                    // 5. Check if session is locked
                    if (session.IsLocked && !isAdmin)
                    {
                        throw new ValidationException("This attendance session is locked and cannot be modified.");
                    }

                    // 6. Prevent duplicate StudentId + AttendanceSessionId
                    var exists = await _repository.AttendanceExistsAsync(request.StudentId, session.AttendanceSessionId);
                    if (exists)
                    {
                        throw new ConflictException($"Attendance has already been marked for Student ID {request.StudentId} in session {session.AttendanceSessionId}.");
                    }

                    // 7. Map and Create the Attendance detail
                    var attendance = _mapper.Map<Attendance>(request);
                    attendance.AttendanceSessionId = session.AttendanceSessionId;

                    var id = await _repository.CreateAttendanceAsync(attendance);

                    // Audit logging
                    var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                    var audit = new AuditLog
                    {
                        UserName = userName,
                        Action = "CREATE",
                        EntityName = "Attendance",
                        EntityId = id,
                        Description = $"Attendance marked for Student ID {request.StudentId} as '{request.Status}'.",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _auditLogRepository.InsertAsync(audit, dbTransaction);

                    await transaction.CommitAsync();
                    _attendanceCache.InvalidateAll();
                    return id;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Creates student attendance records in bulk after master data verification and duplicate checks.
        /// </summary>
        public async Task<int> CreateBulkAttendanceAsync(BulkAttendanceRequest request, bool isAdmin, string userName)
        {
            if (request == null)
            {
                throw new ValidationException("Request body cannot be null.");
            }
            if (request.Students == null || !request.Students.Any())
            {
                throw new ValidationException("Students list cannot be null or empty.");
            }

            // 1. Validate master IDs
            await ValidateMasterDataAsync(request.FacultyId, request.BoardId, request.AcademicYearId, request.AcademicLevelId, request.GroupId, request.SectionId, request.SubjectId);

            // 2. Timetable/Period Checks
            if (request.TimetableId.HasValue)
            {
                var timetable = await _context.Timetables.FindAsync(request.TimetableId.Value);
                if (timetable == null || !timetable.IsPublished)
                {
                    throw new ValidationException($"Published Timetable slot with ID {request.TimetableId.Value} was not found.");
                }
                if (timetable.PeriodId != request.PeriodId)
                {
                    throw new ValidationException($"Timetable slot Period ID {timetable.PeriodId} does not match request Period ID {request.PeriodId}.");
                }
                if (timetable.SectionId != request.SectionId || timetable.SubjectId != request.SubjectId || timetable.StaffId != request.FacultyId)
                {
                    throw new ValidationException("Timetable structural elements do not match request parameters.");
                }
            }
            else
            {
                // Ad-hoc: Validate allocation
                var subjectName = await _context.Subjects.Where(s => s.SubjectId == request.SubjectId).Select(s => s.SubjectName).FirstOrDefaultAsync();
                var allocated = await _context.FacultySubjectAllocations.AnyAsync(a => a.FacultyId == request.FacultyId && a.SubjectId == request.SubjectId);
                if (!allocated)
                {
                    throw new ValidationException($"Faculty member {request.FacultyId} is not allocated to subject {request.SubjectId}.");
                }
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 3. Resolve or Create AttendanceSession
                    var session = await GetOrCreateSessionAsync(request.SectionId, request.PeriodId, request.AttendanceDate, request);

                    // 4. Check if session is locked
                    if (session.IsLocked && !isAdmin)
                    {
                        throw new ValidationException("This attendance session is locked and cannot be modified.");
                    }

                    var attendances = new List<Attendance>();

                    // 5. Loop & validate student belongs to the section, and check duplicates
                    foreach (var studentRequest in request.Students)
                    {
                        var student = await _context.Students.FindAsync(studentRequest.StudentId);
                        if (student == null)
                        {
                            throw new NotFoundException($"Student with ID {studentRequest.StudentId} was not found.");
                        }
                        if (!student.IsActive)
                        {
                            throw new ValidationException($"Student with ID {studentRequest.StudentId} is not active.");
                        }
                        if (student.SectionId != request.SectionId)
                        {
                            throw new ValidationException($"Student with ID {studentRequest.StudentId} does not belong to Section ID {request.SectionId}.");
                        }

                        var exists = await _repository.AttendanceExistsAsync(studentRequest.StudentId, session.AttendanceSessionId);
                        if (exists)
                        {
                            throw new ConflictException($"Attendance has already been marked for Student ID {studentRequest.StudentId} in session {session.AttendanceSessionId}.");
                        }

                        var attendance = _mapper.Map<Attendance>(studentRequest);
                        attendance.AttendanceSessionId = session.AttendanceSessionId;
                        attendance.IsActive = true;
                        attendance.CreatedAt = DateTime.UtcNow;

                        attendances.Add(attendance);
                    }

                    // 6. Bulk insert
                    var affectedRows = await _repository.CreateBulkAttendanceAsync(attendances, session.AttendanceSessionId);

                    // Audit logging: single summary audit entry for the bulk operation
                    var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                    var audit = new AuditLog
                    {
                        UserName = userName,
                        Action = "CREATE",
                        EntityName = "AttendanceSession",
                        EntityId = session.AttendanceSessionId,
                        Description = $"Bulk attendance marked for {request.Students.Count} students in Session ID {session.AttendanceSessionId}.",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _auditLogRepository.InsertAsync(audit, dbTransaction);

                    await transaction.CommitAsync();
                    _attendanceCache.InvalidateAll();
                    return affectedRows;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Updates an existing attendance record after validating its existence.
        /// </summary>
        public async Task<int> UpdateAttendanceAsync(UpdateAttendanceRequest request, bool isAdmin, string userName)
        {
            var existing = await _context.Attendances.AsNoTracking().FirstOrDefaultAsync(a => a.AttendanceId == request.AttendanceId);
            if (existing == null)
            {
                throw new NotFoundException($"Attendance record with ID {request.AttendanceId} was not found.");
            }

            // Verify if the session is locked
            var session = await _context.AttendanceSessions.FindAsync(existing.AttendanceSessionId);
            bool isLocked = session != null && session.IsLocked;
            if (isLocked && !isAdmin)
            {
                throw new ValidationException("This attendance session is locked and cannot be modified.");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var attendance = new Attendance
                    {
                        AttendanceId = request.AttendanceId,
                        AttendanceSessionId = existing.AttendanceSessionId,
                        StudentId = existing.StudentId,
                        Status = request.Status,
                        Remarks = request.Remarks,
                        UpdatedAt = DateTime.UtcNow
                    };

                    var affectedRows = await _repository.UpdateAttendanceAsync(attendance);

                    // Audit logging
                    var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                    var prefix = isLocked ? "[ADMIN OVERRIDE] " : "";
                    var audit = new AuditLog
                    {
                        UserName = userName,
                        Action = "UPDATE",
                        EntityName = "Attendance",
                        EntityId = request.AttendanceId,
                        Description = prefix + $"Status updated from '{existing.Status}' to '{request.Status}'.",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _auditLogRepository.InsertAsync(audit, dbTransaction);

                    await transaction.CommitAsync();
                    _attendanceCache.InvalidateAll();
                    return affectedRows;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Updates multiple existing student attendance records in one bulk operation.
        /// </summary>
        public async Task<int> BulkUpdateAttendanceAsync(BulkUpdateAttendanceRequest request, bool isAdmin, string userName)
        {
            if (request == null)
            {
                throw new ValidationException("Request body cannot be null.");
            }
            if (request.Updates == null || !request.Updates.Any())
            {
                throw new ValidationException("Updates list cannot be null or empty.");
            }

            // Validate the session exists
            var session = await _context.AttendanceSessions.FindAsync(request.AttendanceSessionId);
            if (session == null)
            {
                throw new NotFoundException($"Attendance session with ID {request.AttendanceSessionId} was not found.");
            }

            // Validate the session lock
            if (session.IsLocked && !isAdmin)
            {
                throw new ValidationException("This attendance session is locked and cannot be modified.");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var affectedRows = 0;
                    var descriptionDetails = new List<string>();

                    foreach (var update in request.Updates)
                    {
                        var attendance = await _context.Attendances.FindAsync(update.AttendanceId);
                        if (attendance == null)
                        {
                            throw new NotFoundException($"Attendance record with ID {update.AttendanceId} was not found.");
                        }
                        if (attendance.AttendanceSessionId != request.AttendanceSessionId)
                        {
                            throw new ValidationException($"Attendance record ID {update.AttendanceId} does not belong to session ID {request.AttendanceSessionId}.");
                        }

                        var oldStatus = attendance.Status;
                        attendance.Status = update.Status;
                        attendance.Remarks = update.Remarks;
                        attendance.UpdatedAt = DateTime.UtcNow;

                        affectedRows++;
                        descriptionDetails.Add($"[ID {update.AttendanceId}: {oldStatus} -> {update.Status}]");
                    }

                    await _context.SaveChangesAsync();

                    // Apply the existing audit logging pattern as a single summary entry
                    var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                    var prefix = session.IsLocked ? "[ADMIN OVERRIDE] " : "";
                    var audit = new AuditLog
                    {
                        UserName = userName,
                        Action = "UPDATE",
                        EntityName = "AttendanceSession",
                        EntityId = request.AttendanceSessionId,
                        Description = prefix + $"Bulk attendance update for {request.Updates.Count} records: " + string.Join(", ", descriptionDetails) + ".",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _auditLogRepository.InsertAsync(audit, dbTransaction);

                    await transaction.CommitAsync();
                    _attendanceCache.InvalidateAll();
                    return affectedRows;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Validates existence and updates the active status of an attendance record.
        /// </summary>
        public async Task<int> ChangeAttendanceActiveStatusAsync(int attendanceId, bool isActive, bool isAdmin, string userName)
        {
            var existing = await _context.Attendances.AsNoTracking().FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
            if (existing == null)
            {
                throw new NotFoundException($"Attendance record with ID {attendanceId} was not found.");
            }

            var session = await _context.AttendanceSessions.FindAsync(existing.AttendanceSessionId);
            bool isLocked = session != null && session.IsLocked;
            if (isLocked && !isAdmin)
            {
                throw new ValidationException("This attendance session is locked and cannot be modified.");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    var affectedRows = await _repository.ChangeAttendanceActiveStatusAsync(attendanceId, isActive);

                    // Audit logging
                    var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                    var prefix = isLocked ? "[ADMIN OVERRIDE] " : "";
                    var audit = new AuditLog
                    {
                        UserName = userName,
                        Action = "TOGGLE_STATUS",
                        EntityName = "Attendance",
                        EntityId = attendanceId,
                        Description = prefix + $"Active state changed from '{(existing.IsActive ? "Active" : "Inactive")}' to '{(isActive ? "Active" : "Inactive")}'.",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _auditLogRepository.InsertAsync(audit, dbTransaction);

                    await transaction.CommitAsync();
                    _attendanceCache.InvalidateAll();
                    return affectedRows;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Locks an attendance session, preventing further modifications by Faculty members.
        /// </summary>
        public async Task<bool> LockSessionAsync(int sessionId, int lockedByUserId, string userName)
        {
            var session = await _context.AttendanceSessions.FindAsync(sessionId);
            if (session == null)
            {
                throw new NotFoundException($"Attendance session with ID {sessionId} was not found.");
            }

            if (session.IsLocked)
            {
                throw new ValidationException("This attendance session is already locked.");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    session.IsLocked = true;
                    session.LockedBy = lockedByUserId;
                    session.LockedAt = DateTime.UtcNow;
                    session.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    // Audit logging
                    var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                    var audit = new AuditLog
                    {
                        UserName = userName,
                        Action = "LOCK",
                        EntityName = "AttendanceSession",
                        EntityId = sessionId,
                        Description = $"Attendance session ID {sessionId} was locked.",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _auditLogRepository.InsertAsync(audit, dbTransaction);

                    await transaction.CommitAsync();
                    _attendanceCache.InvalidateAll();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Unlocks an attendance session, allowing modifications.
        /// </summary>
        public async Task<bool> UnlockSessionAsync(int sessionId, string userName)
        {
            var session = await _context.AttendanceSessions.FindAsync(sessionId);
            if (session == null)
            {
                throw new NotFoundException($"Attendance session with ID {sessionId} was not found.");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    session.IsLocked = false;
                    session.LockedBy = null;
                    session.LockedAt = null;
                    session.UpdatedAt = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    // Audit logging
                    var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                    var audit = new AuditLog
                    {
                        UserName = userName,
                        Action = "UNLOCK",
                        EntityName = "AttendanceSession",
                        EntityId = sessionId,
                        Description = $"Attendance session ID {sessionId} was unlocked.",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _auditLogRepository.InsertAsync(audit, dbTransaction);

                    await transaction.CommitAsync();
                    _attendanceCache.InvalidateAll();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Soft deletes an existing attendance record.
        /// </summary>
        public async Task<bool> DeleteAttendanceAsync(int attendanceId, bool isAdmin, string userName)
        {
            var existing = await _context.Attendances.FindAsync(attendanceId);
            if (existing == null)
            {
                throw new NotFoundException($"Attendance record with ID {attendanceId} was not found.");
            }
            if (!existing.IsActive)
            {
                throw new ValidationException($"Attendance record with ID {attendanceId} is already deleted/inactive.");
            }

            var session = await _context.AttendanceSessions.FindAsync(existing.AttendanceSessionId);
            if (session == null)
            {
                throw new NotFoundException($"Attendance session with ID {existing.AttendanceSessionId} was not found.");
            }

            if (session.IsLocked && !isAdmin)
            {
                throw new ValidationException("This attendance session is locked and cannot be modified.");
            }

            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    existing.IsActive = false;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Audit logging
                    var dbTransaction = _context.Database.CurrentTransaction?.GetDbTransaction();
                    var prefix = session.IsLocked ? "[ADMIN OVERRIDE] " : "";
                    var audit = new AuditLog
                    {
                        UserName = userName,
                        Action = "DELETE",
                        EntityName = "Attendance",
                        EntityId = attendanceId,
                        Description = prefix + $"Attendance record ID {attendanceId} was soft deleted.",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _auditLogRepository.InsertAsync(audit, dbTransaction);

                    await transaction.CommitAsync();
                    _attendanceCache.InvalidateAll();
                    return true;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        #endregion

        #region Queries

        /// <summary>
        /// Retrieves a single detailed attendance record by its ID.
        /// </summary>
        public async Task<AttendanceResponse?> GetAttendanceByIdAsync(int attendanceId)
        {
            var key = _attendanceCache.GetCacheKey("GetAttendanceByIdAsync", attendanceId);
            return await _attendanceCache.GetOrCreateAsync(key, async () =>
            {
                var response = await _repository.GetAttendanceByIdAsync(attendanceId);
                if (response == null) return null;

                // Enrich with state fields not returned by the stored procedure, and enforce active checks
                var attendance = await _context.Attendances.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.AttendanceId == attendanceId);
                if (attendance == null || !attendance.IsActive)
                {
                    return null;
                }

                var session = await _context.AttendanceSessions.AsNoTracking()
                    .FirstOrDefaultAsync(s => s.AttendanceSessionId == attendance.AttendanceSessionId);
                if (session == null || !session.IsActive)
                {
                    return null;
                }

                response.IsActive = attendance.IsActive;
                response.IsLocked = session.IsLocked;
                response.LockedBy = session.LockedBy;
                response.LockedAt = session.LockedAt;

                return response;
            });
        }

        /// <summary>
        /// Retrieves a filtered list of attendance records with pagination metadata.
        /// </summary>
        public async Task<PagedResponse<AttendanceListResponse>> GetAttendancesAsync(AttendanceSearchRequest request)
        {
            var key = _attendanceCache.GetCacheKey("GetAttendancesAsync", request);
            return await _attendanceCache.GetOrCreateAsync(key, async () =>
            {
                var totalCount = await _repository.GetAttendancesTotalCountAsync(request);
                var results = (await _repository.GetAttendancesAsync(request)).ToList();

                var currentPage = request.PageNumber <= 0 ? 1 : request.PageNumber;
                var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var pagedResponse = new PagedResponse<AttendanceListResponse>
                {
                    Items = results,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = currentPage,
                    PageSize = pageSize
                };

                if (!results.Any()) return pagedResponse;

                // Batch-fetch attendance state for all returned IDs
                var attendanceIds = results.Select(r => r.AttendanceId).ToList();
                var attendanceEntities = await _context.Attendances.AsNoTracking()
                    .Where(a => attendanceIds.Contains(a.AttendanceId))
                    .Select(a => new { a.AttendanceId, a.IsActive, a.AttendanceSessionId })
                    .ToListAsync();

                var sessionIds = attendanceEntities.Select(a => a.AttendanceSessionId).Distinct().ToList();
                var sessionStates = await _context.AttendanceSessions.AsNoTracking()
                    .Where(s => sessionIds.Contains(s.AttendanceSessionId))
                    .Select(s => new { s.AttendanceSessionId, s.IsLocked })
                    .ToDictionaryAsync(s => s.AttendanceSessionId);

                var attendanceLookup = attendanceEntities.ToDictionary(a => a.AttendanceId);

                foreach (var item in results)
                {
                    if (attendanceLookup.TryGetValue(item.AttendanceId, out var att))
                    {
                        item.IsActive = att.IsActive;
                        if (att.AttendanceSessionId.HasValue && sessionStates.TryGetValue(att.AttendanceSessionId.Value, out var ses))
                        {
                            item.IsLocked = ses.IsLocked;
                        }
                    }
                }

                return pagedResponse;
            });
        }

        /// <summary>
        /// Retrieves students available to mark attendance for the specified search criteria.
        /// </summary>
        public async Task<IEnumerable<StudentAttendanceResponse>> GetStudentsForAttendanceAsync(AttendanceSearchRequest request)
        {
            var key = _attendanceCache.GetCacheKey("GetStudentsForAttendanceAsync", request);
            return await _attendanceCache.GetOrCreateAsync(key, () => _repository.GetStudentsForAttendanceAsync(request));
        }

        /// <summary>
        /// Retrieves statistical summary metrics for the specified filters.
        /// </summary>
        public async Task<AttendanceSummaryResponse> GetAttendanceSummaryAsync(AttendanceSearchRequest request)
        {
            var key = _attendanceCache.GetCacheKey("GetAttendanceSummaryAsync", request);
            return await _attendanceCache.GetOrCreateAsync(key, () => _repository.GetAttendanceSummaryAsync(request));
        }

        /// <summary>
        /// Retrieves attendance percentages and class counts per student for the specified filters.
        /// </summary>
        public async Task<IEnumerable<AttendancePercentageResponse>> GetAttendancePercentageAsync(AttendanceSearchRequest request)
        {
            var key = _attendanceCache.GetCacheKey("GetAttendancePercentageAsync", request);
            return await _attendanceCache.GetOrCreateAsync(key, () => _repository.GetAttendancePercentageAsync(request));
        }

        /// <summary>
        /// Generates a flat report listing attendance details for the specified filters.
        /// </summary>
        public async Task<IEnumerable<AttendanceReportResponse>> GetAttendanceReportAsync(AttendanceSearchRequest request)
        {
            var key = _attendanceCache.GetCacheKey("GetAttendanceReportAsync", request);
            return await _attendanceCache.GetOrCreateAsync(key, () => _repository.GetAttendanceReportAsync(request));
        }

        #endregion

        #region Helper Methods

        private async Task ValidateMasterDataAsync(int facultyId, int boardId, int academicYearId, int academicLevelId, int groupId, int sectionId, int subjectId)
        {
            var facultyExists = await _context.Faculties.AnyAsync(f => f.Id == facultyId && !f.IsDeleted && f.Status == "Active");
            if (!facultyExists)
            {
                throw new NotFoundException($"Active Faculty with ID {facultyId} was not found.");
            }

            var boardExists = await _context.Boards.AnyAsync(b => b.BoardId == boardId && b.IsActive);
            if (!boardExists)
            {
                throw new NotFoundException($"Active Board with ID {boardId} was not found.");
            }

            var academicYearExists = await _context.AcademicYears.AnyAsync(ay => ay.AcademicYearId == academicYearId && ay.IsActive);
            if (!academicYearExists)
            {
                throw new NotFoundException($"Active Academic Year with ID {academicYearId} was not found.");
            }

            var academicLevelExists = await _context.AcademicLevels.AnyAsync(al => al.AcademicLevelId == academicLevelId && al.IsActive);
            if (!academicLevelExists)
            {
                throw new NotFoundException($"Active Academic Level with ID {academicLevelId} was not found.");
            }

            var groupExists = await _context.Groups.AnyAsync(g => g.GroupId == groupId && g.IsActive);
            if (!groupExists)
            {
                throw new NotFoundException($"Active Group with ID {groupId} was not found.");
            }

            var sectionExists = await _context.Sections.AnyAsync(s => s.SectionId == sectionId && s.IsActive);
            if (!sectionExists)
            {
                throw new NotFoundException($"Active Section with ID {sectionId} was not found.");
            }

            var subjectExists = await _context.Subjects.AnyAsync(s => s.SubjectId == subjectId && s.IsActive);
            if (!subjectExists)
            {
                throw new NotFoundException($"Active Subject with ID {subjectId} was not found.");
            }
        }

        private async Task<AttendanceSession> GetOrCreateSessionAsync(int sectionId, int periodId, DateTime date, object requestObj)
        {
            // Find existing session
            var session = await _context.AttendanceSessions.FirstOrDefaultAsync(s => 
                s.SectionId == sectionId && s.PeriodId == periodId && s.AttendanceDate.Date == date.Date);

            if (session != null)
            {
                return session;
            }

            // Create new snapshot session
            session = new AttendanceSession
            {
                AttendanceDate = date.Date,
                PeriodId = periodId,
                IsLocked = false,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            if (requestObj is CreateAttendanceRequest cr)
            {
                session.TimetableId = cr.TimetableId;
                session.SubjectId = cr.SubjectId;
                session.SectionId = cr.SectionId;
                session.FacultyId = cr.FacultyId;
                session.AcademicYearId = cr.AcademicYearId;
                session.AcademicLevelId = cr.AcademicLevelId;
                session.GroupId = cr.GroupId;
                session.BoardId = cr.BoardId;
            }
            else if (requestObj is BulkAttendanceRequest br)
            {
                session.TimetableId = br.TimetableId;
                session.SubjectId = br.SubjectId;
                session.SectionId = br.SectionId;
                session.FacultyId = br.FacultyId;
                session.AcademicYearId = br.AcademicYearId;
                session.AcademicLevelId = br.AcademicLevelId;
                session.GroupId = br.GroupId;
                session.BoardId = br.BoardId;
            }

            await _context.AttendanceSessions.AddAsync(session);
            await _context.SaveChangesAsync();

            return session;
        }

        public async Task<AcademicContextResponse?> GetAcademicContextAsync(int groupId, int sectionId)
        {
            return await _repository.GetAcademicContextAsync(groupId, sectionId);
        }

        public async Task<FacultySubjectDerivationResponse?> GetFacultySubjectAllocationAsync(DateTime date, int groupId, int sectionId, int periodId)
        {
            return await _repository.GetFacultySubjectAllocationAsync(date, groupId, sectionId, periodId);
        }

        public async Task<StudentMonthlyReportResponse> GetStudentMonthlyReportGridAsync(StudentMonthlyReportRequest request)
        {
            return await _repository.GetStudentMonthlyReportGridAsync(request);
        }

        public async Task<byte[]> ExportStudentMonthlyReportToCsvAsync(StudentMonthlyReportRequest request)
        {
            var report = await _repository.GetStudentMonthlyReportGridAsync(request);
            var sb = new StringBuilder();

            // Header line
            var headers = new List<string> { "Roll Number", "Student Name" };
            foreach (var h in report.DayHeaders)
            {
                headers.Add($"{h.DayNumber} ({h.DayName})");
            }
            headers.AddRange(new[] { "Present", "Absent", "Late", "Leave", "Percentage" });
            sb.AppendLine(string.Join(",", headers.Select(h => $"\"{h}\"")));

            // Rows
            foreach (var row in report.StudentRows)
            {
                var line = new List<string>
                {
                    $"\"{row.RollNumber}\"",
                    $"\"{row.StudentName}\""
                };
                foreach (var st in row.DailyStatus)
                {
                    line.Add($"\"{st}\"");
                }
                line.Add($"\"{row.PresentCount}\"");
                line.Add($"\"{row.AbsentCount}\"");
                line.Add($"\"{row.LateCount}\"");
                line.Add($"\"{row.LeaveCount}\"");
                line.Add($"\"{row.Percentage}%\".");
                sb.AppendLine(string.Join(",", line));
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        public async Task<byte[]> ExportStudentMonthlyReportToExcelAsync(StudentMonthlyReportRequest request)
        {
            var report = await _repository.GetStudentMonthlyReportGridAsync(request);
            var dataList = new List<Dictionary<string, object>>();

            foreach (var r in report.StudentRows)
            {
                var dict = new Dictionary<string, object>
                {
                    { "Roll Number", r.RollNumber },
                    { "Student Name", r.StudentName }
                };

                for (int i = 0; i < report.DayHeaders.Count; i++)
                {
                    var dh = report.DayHeaders[i];
                    dict[$"Day {dh.DayNumber} ({dh.DayName})"] = r.DailyStatus.Count > i ? r.DailyStatus[i] : "-";
                }

                dict["Present"] = r.PresentCount;
                dict["Absent"] = r.AbsentCount;
                dict["Late"] = r.LateCount;
                dict["Leave"] = r.LeaveCount;
                dict["Percentage"] = $"{r.Percentage}%";

                dataList.Add(dict);
            }

            using var ms = new MemoryStream();
            await ms.SaveAsAsync(dataList, sheetName: "Student Monthly Report");
            return ms.ToArray();
        }

        #endregion
    }
}
