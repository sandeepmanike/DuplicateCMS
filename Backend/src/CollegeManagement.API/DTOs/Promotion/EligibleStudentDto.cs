namespace CollegeManagement.API.DTOs.Promotion
{
    public class EligibleStudentDto
    {
        public int StudentId { get; set; }

        public string AdmissionNumber { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public int CurrentClassId { get; set; }

        public string CurrentClass { get; set; } = string.Empty;

        public int SectionId { get; set; }

        public string Section { get; set; } = string.Empty;

        // NEW
        public int GroupId { get; set; }

        public string GroupName { get; set; } = string.Empty;

        public int AcademicYearId { get; set; }

        public bool IsEligible { get; set; }
    }
}