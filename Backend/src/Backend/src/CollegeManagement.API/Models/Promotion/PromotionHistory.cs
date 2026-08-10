using System;

namespace CollegeManagement.API.Models.Promotion
{
    public class PromotionHistory
    {
        public int Id { get; set; }

        public int StudentId { get; set; }

        public int FromAcademicYearId { get; set; }

        public int ToAcademicYearId { get; set; }

        public int FromClassId { get; set; }

        public int ToClassId { get; set; }

        public int? FromSectionId { get; set; }

        public int? ToSectionId { get; set; }

        public int? FromGroupId { get; set; }

        public int? ToGroupId { get; set; }

        public DateTime PromotionDate { get; set; }

        public string PromotedBy { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public bool IsRollback { get; set; }

        public DateTime? RollbackDate { get; set; }

        public string? RollbackBy { get; set; }

        public string? RollbackRemarks { get; set; }
    }
}