using System;
using System.Collections.Generic;

namespace CollegeManagement.API.DTOs.Examination.Requests
{
    public class CreateExaminationRequest
    {
        public string ExamCode { get; set; } = string.Empty;
        public string ExamName { get; set; } = string.Empty;
        public int BoardId { get; set; }
        public int GroupId { get; set; }
        public string ExamType { get; set; } = string.Empty;
        public int AcademicYearId { get; set; }
        public string AcademicLevel { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string Status { get; set; } = "Draft";
        public List<int> AllocatedSubjectIds { get; set; } = new();
    }
}