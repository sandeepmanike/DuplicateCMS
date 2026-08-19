namespace CollegeManagement.API.DTOs.Fee
{
    public class FeeStructureResponseDto
    {
        public int FeeStructureId { get; set; }

        public int BoardId { get; set; }

        public string BoardName { get; set; } = null!;

        public int AcademicYearId { get; set; }

        public string AcademicYearName { get; set; } = null!;

        public int AcademicLevelId { get; set; }

        public string AcademicLevelName { get; set; } = null!;

        public int GroupId { get; set; }

        public string GroupName { get; set; } = null!;

        public decimal TotalAmount { get; set; }

        public bool IsActive { get; set; }

        public List<FeeStructureItemDto> Items { get; set; }
            = new();
    }
}