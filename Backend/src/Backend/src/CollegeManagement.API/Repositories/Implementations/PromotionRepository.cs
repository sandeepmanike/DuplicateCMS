using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Promotion;
using CollegeManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly AppDbContext _context;

        public PromotionRepository(AppDbContext context)
        {
            _context = context;
        }

        private IDbConnection Connection =>
            _context.Database.GetDbConnection();

        // ---------------- MOCK DATABASE ----------------

        private static readonly List<EligibleStudentDto> _students = new()
{
    new EligibleStudentDto
    {
        StudentId = 1001,
        AdmissionNumber = "ADM001",
        StudentName = "Ravi Kumar",
        CurrentClassId = 1,
        CurrentClass = "Intermediate First Year",
        SectionId = 1,
        Section = "A",
        GroupId = 1,
        GroupName = "MPC",
        AcademicYearId = 1,
        IsEligible = true
    },

    new EligibleStudentDto
    {
        StudentId = 1002,
        AdmissionNumber = "ADM002",
        StudentName = "Priya Sharma",
        CurrentClassId = 1,
        CurrentClass = "Intermediate First Year",
        SectionId = 1,
        Section = "A",
        GroupId = 2,
        GroupName = "BiPC",
        AcademicYearId = 1,
        IsEligible = true
    },

    new EligibleStudentDto
    {
        StudentId = 1003,
        AdmissionNumber = "ADM003",
        StudentName = "Suresh Reddy",
        CurrentClassId = 1,
        CurrentClass = "Intermediate First Year",
        SectionId = 2,
        Section = "B",
        GroupId = 3,
        GroupName = "CEC",
        AcademicYearId = 1,
        IsEligible = true
    }
};
        private static readonly List<PromotionHistoryDto> _history = new();

        // ------------------------------------------------

        public async Task<List<EligibleStudentDto>> GetEligibleStudentsAsync()
        {
            return await Task.FromResult(
                _students.Where(x => x.IsEligible).ToList());
        }

        public async Task<PromotionResponseDto> PromoteStudentsAsync(PromotionRequestDto dto)
        {
            foreach (var studentId in dto.StudentIds)
            {
                var student = _students.FirstOrDefault(x => x.StudentId == studentId);

                if (student == null)
                    continue;

                if (!student.IsEligible)
                    continue;

                _history.Add(new PromotionHistoryDto
                {
                    PromotionId = _history.Count + 1,
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                    FromClass = student.CurrentClass,
                    ToClass = $"Class {dto.NewClassId}",
                    PromotionDate = DateTime.Now,
                    PromotedBy = "Admin",
                    Remarks = dto.Remarks
                });

                student.CurrentClassId = dto.NewClassId;
                student.CurrentClass = $"Class {dto.NewClassId}";
                student.AcademicYearId = dto.AcademicYearId;
                student.IsEligible = false;
            }

            return await Task.FromResult(new PromotionResponseDto
            {
                Success = true,
                Message = "Students promoted successfully."
            });
        }

        public async Task<PromotionResponseDto> PromoteSingleStudentAsync(int studentId)
        {
            var student = _students.FirstOrDefault(x => x.StudentId == studentId);

            if (student == null)
            {
                return new PromotionResponseDto
                {
                    Success = false,
                    Message = "Student not found."
                };
            }

            if (!student.IsEligible)
            {
                return new PromotionResponseDto
                {
                    Success = false,
                    Message = "Student already promoted."
                };
            }

            _history.Add(new PromotionHistoryDto
            {
                PromotionId = _history.Count + 1,
                StudentId = student.StudentId,
                StudentName = student.StudentName,
                FromClass = student.CurrentClass,
                ToClass = "Intermediate Second Year",
                PromotionDate = DateTime.Now,
                PromotedBy = "Admin",
                Remarks = "Single Promotion"
            });

            student.CurrentClass = "Intermediate Second Year";
            student.CurrentClassId = 2;
            student.IsEligible = false;

            return await Task.FromResult(new PromotionResponseDto
            {
                Success = true,
                Message = "Student promoted successfully."
            });
        }

        public async Task<List<PromotionHistoryDto>> GetPromotionHistoryAsync()
        {
            return await Task.FromResult(_history);
        }

        public async Task<PromotionResponseDto> RollbackPromotionAsync(RollbackPromotionDto dto)
        {
            var promotion = _history.FirstOrDefault(x => x.PromotionId == dto.PromotionId);

            if (promotion == null)
            {
                return new PromotionResponseDto
                {
                    Success = false,
                    Message = "Promotion not found."
                };
            }

            var student = _students.FirstOrDefault(x => x.StudentId == promotion.StudentId);

            if (student != null)
            {
                student.CurrentClass = promotion.FromClass;
                student.CurrentClassId = 1;
                student.IsEligible = true;
            }

            _history.Remove(promotion);

            return await Task.FromResult(new PromotionResponseDto
            {
                Success = true,
                Message = "Promotion rollback completed successfully."
            });
        }

        public async Task<PromotionReportDto> GetPromotionReportAsync()
        {
            return await Task.FromResult(new PromotionReportDto
            {
                TotalStudents = _students.Count,
                PromotedStudents = _history.Count,
                PendingStudents = _students.Count(x => x.IsEligible),
                RollbackStudents = _students.Count - _history.Count - _students.Count(x => x.IsEligible)
            });
        }

        public async Task<bool> UpdateSectionAllocationAsync(SectionAllocationDto dto)
        {
            foreach (var id in dto.StudentIds)
            {
                var student = _students.FirstOrDefault(x => x.StudentId == id);

                if (student != null)
                {
                    student.SectionId = dto.SectionId;

                    student.Section = dto.SectionId switch
                    {
                        1 => "A",
                        2 => "B",
                        3 => "C",
                        4 => "D",
                        _ => "Unknown"
                    };
                }
            }

            return await Task.FromResult(true);
        }
        public async Task<bool> UpdateGroupAllocationAsync(GroupAllocationDto dto)
        {
            foreach (var id in dto.StudentIds)
            {
                var student = _students.FirstOrDefault(x => x.StudentId == id);

                if (student != null)
                {
                    student.GroupId = dto.GroupId;

                    student.GroupName = dto.GroupId switch
                    {
                        1 => "MPC",
                        2 => "BiPC",
                        3 => "CEC",
                        4 => "HEC",
                        _ => "Unknown"
                    };
                }
            }

            return await Task.FromResult(true);
        }
    }
}