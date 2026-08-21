namespace CollegeManagement.API.DTOs.Sections
{
    public class SectionFilterDto
    {
        public string? Board { get; set; }
        public int? AcademicYearId { get; set; }
        public string? Group { get; set; }
        public int? GroupId { get; set; }
        public string? Programme { get; set; }
        public string? Program
        {
            get => Programme;
            set => Programme = value;
        }
        public string? AcademicLevel { get; set; }
        public string? YearOfStudy
        {
            get => AcademicLevel;
            set => AcademicLevel = value;
        }
        public string? SearchTerm { get; set; }
        public string? Search
        {
            get => SearchTerm;
            set => SearchTerm = value;
        }

        public bool? IsActive { get; set; }

        public string? Status
        {
            get => IsActive.HasValue ? (IsActive.Value ? "Active" : "Inactive") : null;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    IsActive = value.Equals("Active", System.StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        public int? InchargeId { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
