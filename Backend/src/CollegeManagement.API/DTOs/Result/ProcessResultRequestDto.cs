using System;
using System.ComponentModel.DataAnnotations;

namespace CollegeManagement.API.DTOs.Result
{
    public class ProcessResultRequestDto
    {
        [Required]
        public int BoardId { get; set; }

        [Required]
        public int AcademicYearId { get; set; }

        [Required]
        public int AcademicLevelId { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public int ExamId { get; set; }

        [Required]
        public DateTime ProcessDate { get; set; }
    }
}