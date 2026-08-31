using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Timetable;
using CollegeManagement.API.Models;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Exports;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;

namespace CollegeManagement.API.Services.Implementations
{
    /// <summary>
    /// Implementation of ITimetableExportService providing Section PDF and Group Excel export capabilities.
    /// </summary>
    public class TimetableExportService : ITimetableExportService
    {
        private readonly AppDbContext _context;
        private readonly ITimetableRepository _timetableRepository;

        public TimetableExportService(
            AppDbContext context,
            ITimetableRepository timetableRepository)
        {
            _context = context;
            _timetableRepository = timetableRepository;
        }

        #region Phase 10F.1 - Section PDF Export
        /// <summary>
        /// Validates academic hierarchy and generates a PDF document for a specific Section timetable.
        /// </summary>
        public async Task<(byte[] PdfBytes, string FileName)> ExportSectionPdfAsync(
            int boardId,
            int academicLevelId,
            int academicYearId,
            int groupId,
            int programId,
            int sectionId)
        {
            // =========================================================================
            // 1. VALIDATE ACADEMIC HIERARCHY (11 INTEGRITY RULES)
            // =========================================================================

            var board = await _context.Boards
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BoardId == boardId);
            if (board == null || !board.IsActive)
            {
                throw new ArgumentException($"Board with ID {boardId} not found or inactive.");
            }

            var level = await _context.AcademicLevels
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.AcademicLevelId == academicLevelId);
            if (level == null || !level.IsActive)
            {
                throw new ArgumentException($"AcademicLevel with ID {academicLevelId} not found or inactive.");
            }

            var year = await _context.AcademicYears
                .AsNoTracking()
                .FirstOrDefaultAsync(y => y.AcademicYearId == academicYearId);
            if (year == null || !year.IsActive)
            {
                throw new ArgumentException($"AcademicYear with ID {academicYearId} not found or inactive.");
            }

            var group = await _context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null || !group.IsActive)
            {
                throw new ArgumentException($"Group with ID {groupId} not found or inactive.");
            }

            var program = await _context.Programs
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProgramId == programId);
            if (program == null || !program.IsActive)
            {
                throw new ArgumentException($"Program with ID {programId} not found or inactive.");
            }

            var section = await _context.Sections
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SectionId == sectionId);
            if (section == null || !section.IsActive)
            {
                throw new ArgumentException($"Section with ID {sectionId} not found or inactive.");
            }

            // Matching checks
            if (section.BoardId.HasValue && section.BoardId.Value != boardId)
            {
                throw new ArgumentException($"Section with ID {sectionId} does not belong to Board ID {boardId}.");
            }

            if (section.AcademicLevelId.HasValue && section.AcademicLevelId.Value != academicLevelId)
            {
                throw new ArgumentException($"Section with ID {sectionId} does not belong to AcademicLevel ID {academicLevelId}.");
            }

            if (section.GroupId.HasValue && section.GroupId.Value != groupId)
            {
                throw new ArgumentException($"Section with ID {sectionId} does not belong to Group ID {groupId}.");
            }

            if (section.ProgramId.HasValue && section.ProgramId.Value != programId)
            {
                throw new ArgumentException($"Section with ID {sectionId} does not belong to Program ID {programId}.");
            }

            if (section.AcademicYearId != academicYearId)
            {
                throw new ArgumentException($"Section with ID {sectionId} does not belong to AcademicYear ID {academicYearId}.");
            }

            // =========================================================================
            // 2. FETCH TIMETABLE SLOTS VIA CANONICAL REPOSITORY
            // =========================================================================

            var queryParams = new TimetableQueryParams
            {
                BoardId = boardId,
                AcademicLevelId = academicLevelId,
                AcademicYearId = academicYearId,
                GroupId = groupId,
                ProgramId = programId,
                SectionId = sectionId,
                PageNumber = 1,
                PageSize = 1000
            };

            var (slots, totalCount) = await _timetableRepository.GetPagedAsync(queryParams);
            var slotsList = slots?.ToList() ?? new List<TimetableResponseDto>();

            if (slotsList.Count == 0)
            {
                throw new KeyNotFoundException("No timetable found for the selected Section.");
            }

            // =========================================================================
            // 3. FETCH CONFIG'D PERIODS (ORDERED BY DISPLAY ORDER / START TIME)
            // =========================================================================

            var allPeriods = await _context.Periods
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.StartTime)
                .ToListAsync();

            var periodColumns = allPeriods.Select(p => new PeriodColumnModel
            {
                PeriodId = p.PeriodId,
                PeriodName = p.PeriodName,
                StartTime = p.StartTime,
                EndTime = p.EndTime,
                DisplayOrder = p.DisplayOrder,
                IsBreak = p.IsBreak
            }).ToList();

            // If no periods in DB, derive from slots
            if (periodColumns.Count == 0)
            {
                periodColumns = slotsList
                    .GroupBy(s => s.PeriodId)
                    .Select(g =>
                    {
                        var first = g.First();
                        return new PeriodColumnModel
                        {
                            PeriodId = first.PeriodId,
                            PeriodName = first.PeriodName,
                            StartTime = first.StartTime,
                            EndTime = first.EndTime,
                            DisplayOrder = first.PeriodId,
                            IsBreak = first.IsBreak
                        };
                    })
                    .OrderBy(p => p.DisplayOrder)
                    .ThenBy(p => p.StartTime)
                    .ToList();
            }

            // =========================================================================
            // 4. GROUP SLOTS BY DAY OF WEEK
            // =========================================================================

            var daysRepresented = slotsList
                .Select(s => s.DayOfWeek)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            if (daysRepresented.Count == 0)
            {
                daysRepresented = Enumerable.Range(1, 6).ToList();
            }

            var daySchedules = new List<DayScheduleModel>();
            foreach (var dayNum in daysRepresented)
            {
                var daySlots = slotsList.Where(s => s.DayOfWeek == dayNum).ToList();
                string dayName = daySlots.FirstOrDefault()?.DayName ?? GetDayName(dayNum);

                var slotMap = new Dictionary<int, TimetableSlotCellModel>();
                foreach (var s in daySlots)
                {
                    slotMap[s.PeriodId] = new TimetableSlotCellModel
                    {
                        SubjectCode = s.SubjectCode,
                        SubjectName = s.SubjectName,
                        StaffName = s.StaffName,
                        StaffEmployeeId = s.StaffEmployeeId,
                        RoomName = s.RoomName,
                        RoomCode = s.RoomCode,
                        IsBreak = s.IsBreak
                    };
                }

                daySchedules.Add(new DayScheduleModel
                {
                    DayOfWeek = dayNum,
                    DayName = dayName,
                    SlotsByPeriodId = slotMap
                });
            }

            // =========================================================================
            // 5. ASSEMBLE PDF MODEL & RENDER DOCUMENT
            // =========================================================================

            var pdfModel = new SectionTimetablePdfModel
            {
                Title = "CLASS TIMETABLE",
                BoardName = board.BoardName,
                AcademicLevelName = level.LevelName,
                AcademicYearName = year.AcademicYearName,
                GroupName = group.GroupName,
                ProgramName = program.ProgramName,
                SectionName = section.SectionName,
                Periods = periodColumns,
                Days = daySchedules
            };

            var document = new SectionTimetablePdfDocument(pdfModel);
            var pdfBytes = document.GeneratePdf();

            // Filename: Timetable_<Group>_<Program>_<Section>.pdf
            var safeGroup = SanitizeFileName(group.GroupName);
            var safeProgram = SanitizeFileName(program.ProgramName);
            var safeSection = SanitizeFileName(section.SectionName);
            var fileName = $"Timetable_{safeGroup}_{safeProgram}_{safeSection}.pdf";

            return (pdfBytes, fileName);
        }
        #endregion

        #region Phase 10F.2 - Group Excel Export
        /// <summary>
        /// Validates academic hierarchy and generates an Excel workbook (.xlsx) for an entire Group,
        /// preserving Program -> Section -> Day -> Period hierarchy across multiple sheets.
        /// </summary>
        public async Task<(byte[] ExcelBytes, string FileName)> ExportGroupExcelAsync(
            int boardId,
            int academicLevelId,
            int academicYearId,
            int groupId)
        {
            // =========================================================================
            // 1. VALIDATE ACADEMIC HIERARCHY
            // =========================================================================

            var board = await _context.Boards
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BoardId == boardId);
            if (board == null || !board.IsActive)
            {
                throw new ArgumentException($"Board with ID {boardId} not found or inactive.");
            }

            var level = await _context.AcademicLevels
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.AcademicLevelId == academicLevelId);
            if (level == null || !level.IsActive)
            {
                throw new ArgumentException($"AcademicLevel with ID {academicLevelId} not found or inactive.");
            }

            var year = await _context.AcademicYears
                .AsNoTracking()
                .FirstOrDefaultAsync(y => y.AcademicYearId == academicYearId);
            if (year == null || !year.IsActive)
            {
                throw new ArgumentException($"AcademicYear with ID {academicYearId} not found or inactive.");
            }

            var group = await _context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group == null || !group.IsActive)
            {
                throw new ArgumentException($"Group with ID {groupId} not found or inactive.");
            }

            if (group.BoardId != boardId)
            {
                throw new ArgumentException($"Group with ID {groupId} does not belong to Board ID {boardId}.");
            }

            if (group.AcademicLevelId != academicLevelId)
            {
                throw new ArgumentException($"Group with ID {groupId} does not belong to AcademicLevel ID {academicLevelId}.");
            }

            // =========================================================================
            // 2. FETCH ALL GROUP TIMETABLE SLOTS VIA CANONICAL REPOSITORY
            // =========================================================================

            var queryParams = new TimetableQueryParams
            {
                BoardId = boardId,
                AcademicLevelId = academicLevelId,
                AcademicYearId = academicYearId,
                GroupId = groupId,
                PageNumber = 1,
                PageSize = 10000
            };

            var (slots, totalCount) = await _timetableRepository.GetPagedAsync(queryParams);
            var slotsList = slots?.ToList() ?? new List<TimetableResponseDto>();

            if (slotsList.Count == 0)
            {
                throw new KeyNotFoundException("No timetable found for the selected Group.");
            }

            // =========================================================================
            // 3. FETCH CONFIGURED PERIODS & ACTIVE SECTIONS
            // =========================================================================

            var allPeriods = await _context.Periods
                .AsNoTracking()
                .Where(p => p.IsActive)
                .OrderBy(p => p.DisplayOrder)
                .ThenBy(p => p.StartTime)
                .ToListAsync();

            var periodColumns = allPeriods.Select(p => new PeriodColumnModel
            {
                PeriodId = p.PeriodId,
                PeriodName = p.PeriodName,
                StartTime = p.StartTime,
                EndTime = p.EndTime,
                DisplayOrder = p.DisplayOrder,
                IsBreak = p.IsBreak
            }).ToList();

            if (periodColumns.Count == 0)
            {
                periodColumns = slotsList
                    .GroupBy(s => s.PeriodId)
                    .Select(g =>
                    {
                        var first = g.First();
                        return new PeriodColumnModel
                        {
                            PeriodId = first.PeriodId,
                            PeriodName = first.PeriodName,
                            StartTime = first.StartTime,
                            EndTime = first.EndTime,
                            DisplayOrder = first.PeriodId,
                            IsBreak = first.IsBreak
                        };
                    })
                    .OrderBy(p => p.DisplayOrder)
                    .ThenBy(p => p.StartTime)
                    .ToList();
            }

            // Fetch active Sections under this Group
            var allSections = await _context.Sections
                .AsNoTracking()
                .Where(s => s.GroupId == groupId && s.BoardId == boardId && s.AcademicLevelId == academicLevelId && s.AcademicYearId == academicYearId && s.IsActive)
                .Include(s => s.ProgramNavigation)
                .OrderBy(s => s.ProgramId)
                .ThenBy(s => s.SectionName)
                .ToListAsync();

            // =========================================================================
            // 4. GROUP DATA BY PROGRAM -> SECTION -> DAY -> PERIOD
            // =========================================================================

            // Determine all distinct programs
            var programMap = new Dictionary<int, string>();

            foreach (var sec in allSections)
            {
                int pKey = sec.ProgramId ?? 0;
                if (!programMap.ContainsKey(pKey))
                {
                    string progName = sec.ProgramNavigation?.ProgramName ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(progName))
                    {
                        progName = sec.ProgramId.HasValue ? $"Program {sec.ProgramId.Value}" : "General";
                    }
                    programMap[pKey] = progName;
                }
            }

            foreach (var slot in slotsList)
            {
                int pKey = slot.ProgramId ?? 0;
                if (!programMap.ContainsKey(pKey))
                {
                    string progName = slot.ProgramName ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(progName))
                    {
                        progName = slot.ProgramId.HasValue ? $"Program {slot.ProgramId.Value}" : "General";
                    }
                    programMap[pKey] = progName;
                }
            }

            var programExcelModels = new List<ProgramTimetableExcelModel>();
            var programSummaries = new List<ProgramOverviewSummaryItem>();

            var standardDays = Enumerable.Range(1, 6).ToList(); // Mon - Sat

            foreach (var kvp in programMap.OrderBy(k => k.Key))
            {
                int pKey = kvp.Key;
                int? progId = pKey == 0 ? (int?)null : pKey;
                string progName = kvp.Value;

                // Find all sections belonging to this Program
                var programSections = allSections.Where(s => (s.ProgramId ?? 0) == pKey).ToList();

                // If sections not in EF DbContext table, derive from slots
                if (programSections.Count == 0)
                {
                    var slotSecIds = slotsList
                        .Where(s => (s.ProgramId ?? 0) == pKey)
                        .Select(s => new { s.SectionId, s.SectionName })
                        .Distinct()
                        .ToList();

                    foreach (var sInfo in slotSecIds)
                    {
                        programSections.Add(new Section
                        {
                            SectionId = sInfo.SectionId,
                            SectionName = sInfo.SectionName,
                            ProgramId = progId,
                            GroupId = groupId,
                            IsActive = true
                        });
                    }
                }

                var sectionBlocks = new List<SectionTimetableExcelBlock>();
                int progSlotCount = 0;

                foreach (var sec in programSections)
                {
                    var secSlots = slotsList.Where(s => s.SectionId == sec.SectionId).ToList();
                    progSlotCount += secSlots.Count;

                    var daySchedules = new List<DayScheduleModel>();
                    if (secSlots.Count > 0)
                    {
                        foreach (var dayNum in standardDays)
                        {
                            var daySlots = secSlots.Where(s => s.DayOfWeek == dayNum).ToList();
                            string dayName = daySlots.FirstOrDefault()?.DayName ?? GetDayName(dayNum);

                            var slotMap = new Dictionary<int, TimetableSlotCellModel>();
                            foreach (var s in daySlots)
                            {
                                slotMap[s.PeriodId] = new TimetableSlotCellModel
                                {
                                    SubjectCode = s.SubjectCode,
                                    SubjectName = s.SubjectName,
                                    StaffName = s.StaffName,
                                    StaffEmployeeId = s.StaffEmployeeId,
                                    RoomName = s.RoomName,
                                    RoomCode = s.RoomCode,
                                    IsBreak = s.IsBreak
                                };
                            }

                            daySchedules.Add(new DayScheduleModel
                            {
                                DayOfWeek = dayNum,
                                DayName = dayName,
                                SlotsByPeriodId = slotMap
                            });
                        }
                    }

                    sectionBlocks.Add(new SectionTimetableExcelBlock
                    {
                        SectionId = sec.SectionId,
                        SectionName = sec.SectionName,
                        HasTimetable = secSlots.Count > 0,
                        Periods = periodColumns,
                        Days = daySchedules
                    });
                }

                programExcelModels.Add(new ProgramTimetableExcelModel
                {
                    ProgramId = progId,
                    ProgramName = progName,
                    Sections = sectionBlocks
                });

                programSummaries.Add(new ProgramOverviewSummaryItem
                {
                    ProgramId = progId,
                    ProgramName = progName,
                    SectionCount = programSections.Count,
                    TotalSlots = progSlotCount
                });
            }

            // =========================================================================
            // 5. ASSEMBLE MODEL & BUILD WORKBOOK
            // =========================================================================

            var excelModel = new GroupTimetableExcelModel
            {
                Title = "GROUP TIMETABLE",
                BoardName = board.BoardName,
                BoardCode = board.BoardCode,
                AcademicLevelName = level.LevelName,
                LevelCode = level.LevelCode,
                AcademicYearName = year.AcademicYearName,
                GroupName = group.GroupName,
                GroupCode = group.GroupCode,
                GeneratedAt = DateTime.Now,
                ProgramSummaries = programSummaries,
                Programs = programExcelModels
            };

            var excelBytes = GroupTimetableExcelBuilder.BuildWorkbook(excelModel);

            // Filename: Timetable_<Group>_<AcademicYear>.xlsx
            var safeGroup = SanitizeFileName(group.GroupName);
            var safeYear = SanitizeFileName(year.AcademicYearName);
            var fileName = $"Timetable_{safeGroup}_{safeYear}.xlsx";

            return (excelBytes, fileName);
        }
        #endregion

        #region Helpers
        private static string GetDayName(int dayOfWeek)
        {
            return dayOfWeek switch
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
        }

        private static string SanitizeFileName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "Timetable";
            var invalidChars = Regex.Escape(new string(Path.GetInvalidFileNameChars()) + " /\\:*?\"<>|");
            var sanitized = Regex.Replace(input, "[" + invalidChars + "]", "_");
            return Regex.Replace(sanitized, "_+", "_").Trim('_');
        }
        #endregion
    }
}
