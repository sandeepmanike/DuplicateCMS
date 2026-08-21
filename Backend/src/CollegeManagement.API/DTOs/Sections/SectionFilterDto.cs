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
        public bool? IsActive { get; set; }
    }
}
