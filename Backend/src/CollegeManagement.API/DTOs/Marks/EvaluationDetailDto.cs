using System;
using System.Collections.Generic;
using CollegeManagement.API.Models.Enums;

namespace CollegeManagement.API.DTOs.Evaluations
{
    public class EvaluationDetailDto
    {
        public string EvaluationId { get; set; } = string.Empty;
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectCode { get; set; } = string.Empty;
        public int? FacultyId { get; set; }
        public string FacultyName { get; set; } = string.Empty;
        public string FacultyCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string ExaminationName { get; set; } = string.Empty;
        public int TotalStudents { get; set; }
        public decimal AverageMarks { get; set; }
        public decimal HighestMarks { get; set; }
        public decimal LowestMarks { get; set; }
        public string Status { get; set; } = string.Empty;
        public EvaluationStatus StatusCode { get; set; }
        public bool IsLocked { get; set; }
        public List<StudentEvaluationMarkRecordDto> Students { get; set; } = new();
        public List<StudentEvaluationMarkRecordDto> MarksList { get => Students; set => Students = value; }
    }

    public class StudentEvaluationMarkRecordDto
    {
        public int MarkId { get; set; }
        public int StudentId { get; set; }
        public string AdmissionNo { get; set; } = string.Empty;
        public string RollNo { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public decimal Internal { get; set; }
        public decimal InternalMarks { get => Internal; set => Internal = value; }
        public decimal Practical { get; set; }
        public decimal PracticalMarks { get => Practical; set => Practical = value; }
        public decimal Theory { get; set; }
        public decimal TheoryMarks { get => Theory; set => Theory = value; }
        public decimal TotalMarks { get; set; }
        public bool IsAbsent { get; set; }
        public string? Remarks { get; set; }
    }

    public class UpdateEvaluationMarksRequestDto
    {
        public List<StudentMarkUpdateItemDto> StudentMarks { get; set; } = new();
        public List<StudentMarkUpdateItemDto> Students { get => StudentMarks; set => StudentMarks = value; }
    }

    public class StudentMarkUpdateItemDto
    {
        public int? MarkId { get; set; }
        public int StudentId { get; set; }
        public decimal? Internal { get; set; }
        public decimal? Practical { get; set; }
        public decimal? Theory { get; set; }
        public bool? IsAbsent { get; set; }
        public string? Remarks { get; set; }
    }

    public class RejectEvaluationRequestDto
    {
        public string? Remarks { get; set; }
        public string? Reason { get; set; }
        public bool NotifyFaculty { get; set; } = true;
    }
}