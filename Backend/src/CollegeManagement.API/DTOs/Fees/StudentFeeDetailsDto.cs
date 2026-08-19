namespace CollegeManagement.API.DTOs.Fee
{
    public class StudentFeeDetailsDto
    {
        public int StudentFeeAssignmentId { get; set; }

        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;

        public int AdmissionId { get; set; }
        public string AdmissionNo { get; set; } = string.Empty;

        public int BoardId { get; set; }
        public string BoardName { get; set; } = string.Empty;

        public int AcademicYearId { get; set; }
        public string AcademicYearName { get; set; } = string.Empty;

        public int AcademicLevelId { get; set; }
        public string AcademicLevelName { get; set; } = string.Empty;

        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;

        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;

        public int FeeStructureId { get; set; }

        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; } = string.Empty;

        public decimal OriginalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal ScholarshipAmount { get; set; }
        public decimal PayableAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal BalanceAmount { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}