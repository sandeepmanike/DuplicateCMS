using CollegeManagement.API.Models; 

namespace CollegeManagement.API.Models
{
    public class FeeStructure
    {
        public int Id { get; set; }
        public int BoardId { get; set; }
        public int AcademicYearId { get; set; }
        public int GroupId { get; set; }

        public string FeeType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsActive { get; set; }

        // Navigation Properties
        public AcademicYear AcademicYear { get; set; } = null!;


        public ICollection<FeeCollection> FeeCollections { get; set; } = new List<FeeCollection>();
        
        public Board Board { get; set; } = null!;
        public Group  Group { get; set; } = null!;

    }
}