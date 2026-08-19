using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CollegeManagement.API.DTOs.Evaluations;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Models;
using CollegeManagement.API.Models.Enums;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace CollegeManagement.API.Services.Implementations
{
    public class EvaluationService : IEvaluationService
    {
        private readonly IMarksRepository _marksRepository;
        private readonly ILogger<EvaluationService> _logger;

        public EvaluationService(IMarksRepository marksRepository, ILogger<EvaluationService> logger)
        {
            _marksRepository = marksRepository;
            _logger = logger;
        }

        // --- 1. Search & Filter Evaluations ---
        public async Task<List<EvaluationListDto>> SearchEvaluationsAsync(EvaluationFilterDto filter)
        {
            var (items, _) = await GetFilteredEvaluationsAsync(filter);
            return items.ToList();
        }

        public async Task<(IEnumerable<EvaluationListDto> Items, int TotalCount)> GetFilteredEvaluationsAsync(EvaluationFilterDto filter)
        {
            filter.PageSize = filter.PageSize <= 0 ? 10000 : (filter.PageSize <= 50 ? 10000 : filter.PageSize);
            var rawMarks = await _marksRepository.GetFilteredEvaluationsAsync(filter);
            var totalCount = await _marksRepository.GetFilteredEvaluationsCountAsync(filter);

            var groupedEvaluations = rawMarks
                .GroupBy(m => new { m.SubjectId, m.SectionId, m.ExaminationId })
                .Select((g, index) =>
                {
                    var first = g.First();
                    var total = g.Count();
                    var present = g.Count(m => !m.IsAbsent);
                    var absent = total - present;
                    var avg = total > 0 ? (decimal)g.Average(m => m.TotalMarks) : 0m;
                    var max = total > 0 ? (decimal)g.Max(m => m.TotalMarks) : 0m;
                    var min = total > 0 ? (decimal)g.Min(m => m.TotalMarks) : 0m;

                    var facName = first.Faculty != null ? $"{first.Faculty.FirstName} {first.Faculty.LastName}".Trim() : "Unassigned";
                    var facCode = first.Faculty != null ? (!string.IsNullOrWhiteSpace(first.Faculty.EmployeeId) ? first.Faculty.EmployeeId : $"FAC{first.Faculty.Id:0000}") : "FAC1001";

                    return new EvaluationListDto
                    {
                        EvaluationId = $"{first.SubjectId}_{first.SectionId}_{first.ExaminationId}",
                        EvaluationKey = index + 1,
                        SubjectId = first.SubjectId,
                        SubjectCode = first.Subject?.SubjectCode ?? $"SUB{first.SubjectId:000}",
                        SubjectName = first.Subject?.SubjectName ?? $"Subject #{first.SubjectId}",
                        FacultyId = first.FacultyId,
                        FacultyName = facName,
                        FacultyCode = facCode,
                        BoardName = !string.IsNullOrWhiteSpace(first.Board) ? first.Board : "BIE Telangana",
                        AcademicYear = first.AcademicYearId > 0 ? first.AcademicYearId.ToString() : "1",
                        GroupName = first.GroupNavigation?.GroupName ?? "MPC",
                        SectionName = first.SectionNavigation?.SectionName ?? "Section A",
                        ExaminationName = first.Examination?.ExamName ?? "Semester I",
                        TotalStudents = total,
                        PresentStudents = present,
                        AbsentStudents = absent,
                        AverageMarks = Math.Round(avg, 2),
                        ObtainedMarks = Math.Round(avg, 0),
                        TotalMarks = 100,
                        HighestMarks = max,
                        LowestMarks = min,
                        Status = first.Status.ToString().ToUpperInvariant(),
                        StatusCode = first.Status,
                        IsLocked = first.IsLocked,
                        RejectionReason = first.Status == EvaluationStatus.REJECTED ? first.Remarks : null,
                        Remarks = first.Remarks,
                        LastSubmittedAt = first.UpdatedAt ?? first.CreatedAt
                    };
                })
                .ToList();

            return (groupedEvaluations, totalCount);
        }

        // --- 2. Subject Breakdown & Student Marks ---
        public async Task<EvaluationDetailDto?> GetEvaluationDetailAsync(int subjectId, int sectionId, int examinationId)
        {
            var marks = (await _marksRepository.GetEvaluationMarksListAsync(subjectId, sectionId, examinationId)).ToList();
            if (!marks.Any()) return null;

            var first = marks.First();
            var total = marks.Count;
            var avg = total > 0 ? (decimal)marks.Average(m => m.TotalMarks) : 0m;
            var max = total > 0 ? (decimal)marks.Max(m => m.TotalMarks) : 0m;
            var min = total > 0 ? (decimal)marks.Min(m => m.TotalMarks) : 0m;

            var facName = first.Faculty != null ? $"{first.Faculty.FirstName} {first.Faculty.LastName}".Trim() : "Unassigned";
            var facCode = first.Faculty != null ? (!string.IsNullOrWhiteSpace(first.Faculty.EmployeeId) ? first.Faculty.EmployeeId : $"FAC{first.Faculty.Id:0000}") : "FAC1001";

            return new EvaluationDetailDto
            {
                EvaluationId = $"{subjectId}_{sectionId}_{examinationId}",
                SubjectId = subjectId,
                SubjectName = first.Subject?.SubjectName ?? $"Subject #{subjectId}",
                SubjectCode = first.Subject?.SubjectCode ?? $"SUB{subjectId:000}",
                FacultyId = first.FacultyId,
                FacultyName = facName,
                FacultyCode = facCode,
                GroupName = first.GroupNavigation?.GroupName ?? "MPC",
                SectionName = first.SectionNavigation?.SectionName ?? "Section A",
                ExaminationName = first.Examination?.ExamName ?? "Semester I",
                TotalStudents = total,
                AverageMarks = Math.Round(avg, 2),
                HighestMarks = max,
                LowestMarks = min,
                Status = first.Status.ToString().ToUpperInvariant(),
                StatusCode = first.Status,
                IsLocked = first.IsLocked,
                Students = marks.Select(m => new StudentEvaluationMarkRecordDto
                {
                    MarkId = m.MarkId,
                    StudentId = m.StudentId,
                    AdmissionNo = m.Student?.AdmissionNo ?? (!string.IsNullOrEmpty(m.RollNo) ? m.RollNo : "N/A"),
                    RollNo = !string.IsNullOrEmpty(m.RollNo) ? m.RollNo : (m.Student?.RollNo ?? $"ROLL{m.StudentId:000}"),
                    StudentName = !string.IsNullOrEmpty(m.StudentName) ? m.StudentName : (m.Student?.StudentName ?? "Student"),
                    Internal = m.InternalMarks,
                    Practical = m.PracticalMarks,
                    Theory = m.TheoryMarks,
                    TotalMarks = m.TotalMarks,
                    IsAbsent = m.IsAbsent,
                    Remarks = m.Remarks
                }).ToList()
            };
        }

        public async Task<EvaluationDetailDto?> GetEvaluationByCompositeIdAsync(string evaluationId)
        {
            var (subjectId, sectionId, examinationId) = ParseEvaluationId(evaluationId);
            if (subjectId <= 0) return null;
            return await GetEvaluationDetailAsync(subjectId, sectionId, examinationId);
        }

        // --- 3. Status Transitions (Verify / Approve / Reject / Restore / Approve All) ---
        public async Task<bool> UpdateEvaluationStatusAsync(int subjectId, int sectionId, int examinationId, EvaluationStatus targetStatus, int userId)
        {
            return await _marksRepository.UpdateEvaluationStatusAsync(subjectId, sectionId, examinationId, targetStatus, userId);
        }

        public async Task<bool> UpdateEvaluationStatusByCompositeIdAsync(string evaluationId, EvaluationStatus targetStatus, int userId, string? remarks = null)
        {
            var (subjectId, sectionId, examinationId) = ParseEvaluationId(evaluationId);
            if (subjectId <= 0) return false;
            return await _marksRepository.UpdateEvaluationStatusAsync(subjectId, sectionId, examinationId, targetStatus, userId, remarks);
        }

        public async Task<(bool Success, int Count)> VerifyAllEvaluationsAsync(EvaluationFilterDto filter, int userId)
        {
            var verifyFilter = new EvaluationFilterDto
            {
                BoardId = filter.BoardId,
                AcademicYearId = filter.AcademicYearId,
                ProgramId = filter.ProgramId,
                GroupId = filter.GroupId,
                SectionId = filter.SectionId,
                ExaminationId = filter.ExaminationId,
                SubjectId = filter.SubjectId,
                FacultyId = filter.FacultyId,
                Status = EvaluationStatus.SUBMITTED,
                PageSize = 10000
            };

            var rawMarks = (await _marksRepository.GetFilteredEvaluationsAsync(verifyFilter))
                .Where(m => !m.IsLocked && m.Status == EvaluationStatus.SUBMITTED)
                .ToList();

            if (!rawMarks.Any()) return (false, 0);

            var groups = rawMarks.GroupBy(m => new { m.SubjectId, m.SectionId, m.ExaminationId }).ToList();
            int verifiedCount = 0;

            foreach (var g in groups)
            {
                var success = await _marksRepository.UpdateEvaluationStatusAsync(
                    g.Key.SubjectId, g.Key.SectionId, g.Key.ExaminationId, EvaluationStatus.VERIFIED, userId);
                if (success) verifiedCount++;
            }

            return (verifiedCount > 0, verifiedCount);
        }

        public async Task<bool> ApproveAllEvaluationsAsync(EvaluationFilterDto filter, int userId)
        {
            var rawMarks = (await _marksRepository.GetFilteredEvaluationsAsync(filter)).ToList();
            if (!rawMarks.Any()) return false;

            var groups = rawMarks.GroupBy(m => new { m.SubjectId, m.SectionId, m.ExaminationId });
            foreach (var g in groups)
            {
                await _marksRepository.UpdateEvaluationStatusAsync(g.Key.SubjectId, g.Key.SectionId, g.Key.ExaminationId, EvaluationStatus.APPROVED, userId);
            }
            return true;
        }

        // --- 4. Admin Edit Student Marks ---
        public async Task<bool> UpdateStudentMarksByCompositeIdAsync(string evaluationId, List<StudentMarkUpdateItemDto> updates, int userId)
        {
            var (subjectId, sectionId, examinationId) = ParseEvaluationId(evaluationId);
            if (subjectId <= 0) return false;
            return await _marksRepository.UpdateStudentMarksAsync(subjectId, sectionId, examinationId, updates, userId);
        }

        // --- 5. Student Analysis Performance Matrix ---
        public async Task<List<StudentSubjectMatrixDto>> GetStudentAnalysisMatrixAsync(
            int? academicYearId, int? groupId, int? sectionId, int? examinationId, int? boardId = null, int? academicLevelId = null)
        {
            var filter = new EvaluationFilterDto
            {
                AcademicYearId = academicYearId,
                GroupId = groupId,
                SectionId = sectionId,
                ExaminationId = examinationId,
                BoardId = boardId,
                PageSize = 10000
            };

            var rawMarks = (await _marksRepository.GetFilteredEvaluationsAsync(filter)).ToList();

            var groupedByStudent = rawMarks
                .GroupBy(m => m.StudentId)
                .Select(g =>
                {
                    var first = g.First();
                    var rollNo = !string.IsNullOrEmpty(first.RollNo) ? first.RollNo : (first.Student?.RollNo ?? $"ROLL{first.StudentId:000}");
                    var studentName = !string.IsNullOrEmpty(first.StudentName) ? first.StudentName : (first.Student?.StudentName ?? "Student");

                    var subjectDict = new Dictionary<string, decimal>();
                    var subjectsList = new List<StudentSubjectMarkItemDto>();
                    decimal grandTotal = 0;

                    foreach (var mark in g)
                    {
                        var subjectName = mark.Subject?.SubjectName ?? $"Subject-{mark.SubjectId}";
                        var subjectCode = mark.Subject?.SubjectCode ?? $"SUB{mark.SubjectId:000}";
                        subjectDict[subjectName] = mark.TotalMarks;
                        subjectDict[mark.SubjectId.ToString()] = mark.TotalMarks;
                        subjectsList.Add(new StudentSubjectMarkItemDto
                        {
                            SubjectId = mark.SubjectId,
                            SubjectName = subjectName,
                            SubjectCode = subjectCode,
                            Marks = mark.TotalMarks
                        });
                        grandTotal += mark.TotalMarks;
                    }

                    var count = g.Count();
                    var maxPossible = count * 100;
                    var percentage = maxPossible > 0 ? Math.Round((grandTotal / maxPossible) * 100, 2) : 0m;
                    string grade = CalculateGrade(percentage);
                    string result = g.Any(m => m.IsAbsent || m.TotalMarks < m.PassingMarks) ? "FAIL" : "PASS";

                    return new StudentSubjectMatrixDto
                    {
                        StudentId = g.Key,
                        RollNo = rollNo,
                        StudentName = studentName,
                        TotalMarks = grandTotal,
                        MaxTotal = maxPossible,
                        Percentage = percentage,
                        Grade = grade,
                        Result = result,
                        Subjects = subjectsList,
                        SubjectMarks = subjectDict
                    };
                })
                .OrderBy(s => s.RollNo)
                .ToList();

            return groupedByStudent;
        }

        public async Task<StudentAnalysisDetailDto?> GetStudentAnalysisDetailAsync(
            int studentId,
            int? examinationId = null,
            int? academicYearId = null,
            int? groupId = null,
            int? sectionId = null,
            int? boardId = null,
            int? academicLevelId = null)
        {
            var filter = new EvaluationFilterDto
            {
                StudentId = studentId,
                ExaminationId = examinationId,
                AcademicYearId = academicYearId,
                GroupId = groupId,
                SectionId = sectionId,
                BoardId = boardId,
                PageSize = 1000
            };

            var studentMarks = (await _marksRepository.GetFilteredEvaluationsAsync(filter)).ToList();
            if (!studentMarks.Any())
            {
                studentMarks = (await _marksRepository.GetByStudentAsync(studentId)).ToList();
                if (examinationId.HasValue && examinationId.Value > 0)
                {
                    studentMarks = studentMarks.Where(m => m.ExaminationId == examinationId.Value).ToList();
                }
                if (!studentMarks.Any()) return null;
            }

            var first = studentMarks.First();
            var rollNo = !string.IsNullOrEmpty(first.RollNo) ? first.RollNo : (first.Student?.RollNo ?? $"ROLL{first.StudentId:000}");
            var studentName = !string.IsNullOrEmpty(first.StudentName) ? first.StudentName : (first.Student?.StudentName ?? "Student");
            var groupName = first.GroupNavigation?.GroupName ?? "N/A";
            var sectionName = first.SectionNavigation?.SectionName ?? "N/A";
            var examName = first.Examination?.ExamName ?? "N/A";

            var subjectsList = new List<StudentSubjectAnalysisDetailItemDto>();
            decimal grandTotal = 0;

            foreach (var mark in studentMarks)
            {
                var subjectName = mark.Subject?.SubjectName ?? $"Subject-{mark.SubjectId}";
                var subjectCode = mark.Subject?.SubjectCode ?? $"SUB{mark.SubjectId:000}";
                var total = mark.TotalMarks;

                subjectsList.Add(new StudentSubjectAnalysisDetailItemDto
                {
                    SubjectId = mark.SubjectId,
                    SubjectName = subjectName,
                    SubjectCode = subjectCode,
                    Internal = mark.InternalMarks,
                    Practical = mark.PracticalMarks > 0 ? mark.PracticalMarks : null,
                    Theory = mark.TheoryMarks,
                    Total = total,
                    PassingMarks = mark.PassingMarks > 0 ? mark.PassingMarks : 35,
                    IsAbsent = mark.IsAbsent,
                    Remarks = mark.Remarks
                });

                grandTotal += total;
            }

            var count = studentMarks.Count;
            var maxPossible = count * 100;
            var percentage = maxPossible > 0 ? Math.Round((grandTotal / maxPossible) * 100, 2) : 0m;
            string grade = CalculateGrade(percentage);

            int? rank = null;
            var effectiveExamId = examinationId ?? first.ExaminationId;
            var effectiveSectionId = sectionId ?? first.SectionId;
            var effectiveGroupId = groupId ?? first.GroupId;

            if (effectiveExamId > 0 && (effectiveSectionId > 0 || effectiveGroupId > 0))
            {
                try
                {
                    var cohortFilter = new EvaluationFilterDto
                    {
                        ExaminationId = effectiveExamId,
                        SectionId = effectiveSectionId > 0 ? effectiveSectionId : null,
                        GroupId = effectiveGroupId > 0 ? effectiveGroupId : null,
                        AcademicYearId = academicYearId ?? first.AcademicYearId,
                        BoardId = boardId ?? first.BoardId,
                        PageSize = 10000
                    };
                    var cohortMarks = (await _marksRepository.GetFilteredEvaluationsAsync(cohortFilter)).ToList();
                    var cohortTotals = cohortMarks
                        .GroupBy(m => m.StudentId)
                        .Select(g => new { StudentId = g.Key, Total = g.Sum(m => m.TotalMarks) })
                        .OrderByDescending(x => x.Total)
                        .ToList();

                    var rankIndex = cohortTotals.FindIndex(x => x.StudentId == studentId);
                    if (rankIndex >= 0)
                    {
                        rank = rankIndex + 1;
                    }
                }
                catch
                {
                    rank = 1;
                }
            }

            return new StudentAnalysisDetailDto
            {
                StudentId = studentId,
                RollNo = rollNo,
                StudentName = studentName,
                GroupName = groupName,
                SectionName = sectionName,
                ExamName = examName,
                TotalMarks = grandTotal,
                MaxMarks = maxPossible,
                Percentage = percentage,
                Grade = grade,
                Rank = rank,
                Subjects = subjectsList
            };
        }

        public async Task<List<StudentSubjectMatrixDto>> GetStudentSubjectMatrixAsync(int sectionId, int examinationId)
        {
            return await GetStudentAnalysisMatrixAsync(null, null, sectionId, examinationId);
        }

        public async Task<SubjectAnalysisDto?> GetSubjectPerformanceAnalysisAsync(int subjectId, int? sectionId, int? examinationId)
        {
            var marks = (await _marksRepository.GetSubjectStudentMarksAsync(subjectId, sectionId, examinationId)).ToList();
            if (!marks.Any()) return null;

            var first = marks.First();
            var total = marks.Count;
            var passed = marks.Count(m => !m.IsAbsent && m.TotalMarks >= m.PassingMarks);
            var failed = total - passed;
            var passPct = total > 0 ? ((decimal)passed / total) * 100 : 0m;
            var avg = total > 0 ? (decimal)marks.Average(m => m.TotalMarks) : 0m;
            var max = total > 0 ? (decimal)marks.Max(m => m.TotalMarks) : 0m;
            var min = total > 0 ? (decimal)marks.Min(m => m.TotalMarks) : 0m;

            return new SubjectAnalysisDto
            {
                SubjectId = subjectId,
                SubjectCode = first.Subject?.SubjectCode ?? "N/A",
                SubjectName = first.Subject?.SubjectName ?? "N/A",
                FacultyName = first.Faculty != null ? $"{first.Faculty.FirstName} {first.Faculty.LastName}".Trim() : "Unassigned",
                TotalStudents = total,
                TotalPassed = passed,
                TotalFailed = failed,
                PassPercentage = Math.Round(passPct, 2),
                AverageMarks = Math.Round(avg, 2),
                HighestMarks = max,
                LowestMarks = min,
                StudentsPerformance = marks.Select(m => new SubjectStudentPerformanceDto
                {
                    StudentId = m.StudentId,
                    RollNo = string.IsNullOrEmpty(m.RollNo) ? "N/A" : m.RollNo,
                    StudentName = string.IsNullOrEmpty(m.StudentName) ? "Student" : m.StudentName,
                    SectionName = m.SectionNavigation?.SectionName ?? "N/A",
                    ObtainedMarks = m.TotalMarks,
                    IsAbsent = m.IsAbsent,
                    ResultStatus = m.IsAbsent ? "Absent" : (m.TotalMarks >= m.PassingMarks ? "Pass" : "Fail")
                }).ToList()
            };
        }

        // --- 6. Dropdown Hierarchy Helpers ---
        public async Task<IEnumerable<dynamic>> GetGroupSectionsAsync(int groupId)
        {
            return await _marksRepository.GetGroupSectionsAsync(groupId);
        }

        public async Task<IEnumerable<dynamic>> GetGroupSubjectsAsync(int groupId)
        {
            return await _marksRepository.GetGroupSubjectsAsync(groupId);
        }

        // --- 7. Faculty Entry & Governance Overrides ---
        public async Task<bool> SaveFacultyMarksEntryAsync(FacultyMarksEntryDto dto)
        {
            if (dto == null || dto.StudentMarks == null || !dto.StudentMarks.Any())
            {
                throw new ValidationException("Student marks list cannot be empty.");
            }

            IEnumerable<Mark> existingMarksList;
            try
            {
                existingMarksList = await _marksRepository.GetSubjectStudentMarksAsync(
                    dto.SubjectId, null, dto.ExaminationId);
            }
            catch
            {
                existingMarksList = new List<Mark>();
            }

            var existingMarks = existingMarksList.ToList();
            if (existingMarks.Any(m => m.IsLocked))
            {
                throw new ConflictException("Evaluation entries for this subject and section are locked by Administrator.");
            }

            var targetStatus = dto.SubmitForEvaluation ? EvaluationStatus.SUBMITTED : EvaluationStatus.SUBMITTED;
            var now = DateTime.UtcNow;

            foreach (var item in dto.StudentMarks)
            {
                var markEntity = existingMarks.FirstOrDefault(m => m.StudentId == item.StudentId);
                if (markEntity == null)
                {
                    markEntity = await _marksRepository.GetByExamSubjectStudentAsync(dto.ExaminationId, dto.SubjectId, item.StudentId);
                }

                if (markEntity != null)
                {
                    markEntity.BoardId = dto.BoardId > 0 ? dto.BoardId : markEntity.BoardId;
                    markEntity.AcademicYearId = dto.AcademicYearId;
                    markEntity.AcademicLevelId = dto.AcademicLevelId > 0 ? dto.AcademicLevelId : markEntity.AcademicLevelId;
                    markEntity.GroupId = dto.GroupId;
                    markEntity.SectionId = dto.SectionId;
                    markEntity.InternalMarks = (int)item.InternalMarks;
                    markEntity.PracticalMarks = (int)item.PracticalMarks;
                    markEntity.TheoryMarks = (int)item.TheoryMarks;
                    markEntity.TotalMarks = (int)(item.InternalMarks + item.PracticalMarks + item.TheoryMarks);
                    markEntity.IsAbsent = item.IsAbsent;
                    markEntity.Remarks = item.Remarks;
                    if (dto.FacultyId > 0) markEntity.FacultyId = dto.FacultyId;
                    markEntity.Status = targetStatus;
                    markEntity.IsActive = true;
                    markEntity.UpdatedAt = now;

                    if (!string.IsNullOrEmpty(item.RollNo)) markEntity.RollNo = item.RollNo;
                    if (!string.IsNullOrEmpty(item.StudentName)) markEntity.StudentName = item.StudentName;

                    markEntity.Student = null;
                    markEntity.Subject = null;
                    markEntity.Faculty = null;
                    markEntity.SectionNavigation = null;
                    markEntity.BoardNavigation = null;
                    markEntity.AcademicYear = null;
                    markEntity.AcademicLevelNavigation = null;
                    markEntity.GroupNavigation = null;
                    markEntity.Examination = null;

                    await _marksRepository.UpdateAsync(markEntity);
                }
                else
                {
                    var newMark = new Mark
                    {
                        BoardId = dto.BoardId > 0 ? dto.BoardId : null,
                        Board = !string.IsNullOrEmpty(dto.Board) ? dto.Board : "BIE Telangana",
                        AcademicYearId = dto.AcademicYearId,
                        AcademicLevelId = dto.AcademicLevelId > 0 ? dto.AcademicLevelId : 1,
                        AcademicLevel = !string.IsNullOrEmpty(dto.AcademicLevel) ? dto.AcademicLevel : "Intermediate First Year",
                        GroupId = dto.GroupId,
                        SectionId = dto.SectionId,
                        ExaminationId = dto.ExaminationId,
                        SubjectId = dto.SubjectId,
                        FacultyId = dto.FacultyId > 0 ? dto.FacultyId : null,
                        StudentId = item.StudentId,
                        RollNo = !string.IsNullOrEmpty(item.RollNo) ? item.RollNo : "UG2026001",
                        StudentName = !string.IsNullOrEmpty(item.StudentName) ? item.StudentName : "Student",
                        InternalMarks = (int)item.InternalMarks,
                        PracticalMarks = (int)item.PracticalMarks,
                        TheoryMarks = (int)item.TheoryMarks,
                        TotalMarks = (int)(item.InternalMarks + item.PracticalMarks + item.TheoryMarks),
                        PassingMarks = 35,
                        IsAbsent = item.IsAbsent,
                        Remarks = item.Remarks,
                        Status = targetStatus,
                        IsActive = true,
                        CreatedAt = now
                    };

                    await _marksRepository.AddAsync(newMark);
                }
            }

            await _marksRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleEvaluationLockAsync(LockEvaluationDto dto)
        {
            return await _marksRepository.ToggleEvaluationLockAsync(dto.SubjectId, dto.SectionId, dto.ExaminationId, dto.IsLocked);
        }

        public async Task<bool> OverrideEvaluationStatusAsync(OverrideEvaluationStatusDto dto, int userId)
        {
            _logger.LogWarning("Super Admin (UserId: {UserId}) overridden evaluation status for Subject: {SubjectId}, Section: {SectionId}, Exam: {ExamId} to {Status}. Reason: {Reason}",
                userId, dto.SubjectId, dto.SectionId, dto.ExaminationId, dto.TargetStatus, dto.Reason);

            return await _marksRepository.UpdateEvaluationStatusAsync(dto.SubjectId, dto.SectionId, dto.ExaminationId, dto.TargetStatus, userId);
        }

        public async Task<bool> ExecuteGlobalApprovalAsync(CollegeManagement.API.DTOs.Marks.GlobalApprovalRequestDto dto, int userId)
        {
            _logger.LogInformation("Executing Global Approval by User {UserId} for ExamId: {ExamId}, GroupId: {GroupId}, AcademicYearId: {AcademicYearId}",
                userId, dto.ExaminationId, dto.GroupId, dto.AcademicYearId);

            return await _marksRepository.ExecuteGlobalApprovalAsync(dto, userId);
        }

        // --- Private Helpers ---
        private (int subjectId, int sectionId, int examinationId) ParseEvaluationId(string evaluationId)
        {
            if (string.IsNullOrWhiteSpace(evaluationId)) return (0, 0, 0);

            var parts = evaluationId.Split(new[] { '_', '-' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 3 &&
                int.TryParse(parts[0], out int s) &&
                int.TryParse(parts[1], out int sec) &&
                int.TryParse(parts[2], out int e))
            {
                return (s, sec, e);
            }

            if (int.TryParse(evaluationId, out int singleId))
            {
                return (singleId, 0, 0);
            }

            return (0, 0, 0);
        }

        private string CalculateGrade(decimal percentage)
        {
            if (percentage >= 90) return "A+";
            if (percentage >= 80) return "A";
            if (percentage >= 70) return "B+";
            if (percentage >= 60) return "B";
            if (percentage >= 50) return "C";
            if (percentage >= 35) return "D";
            return "F";
        }
    }
}