using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models.Staff
{
    [Table("StaffSubjectAllocations")]
    public class StaffSubjectAllocation
    {


        [Key]
        public int Id { get; set; }

        [Required]
        public int StaffId { get; set; }

        [NotMapped]
        public int FacultyId
        {
            get => StaffId;
            set => StaffId = value;
        }

        [Required]
        public int SubjectId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(StaffId))]
        public Staff Staff { get; set; } = null!;

        [ForeignKey(nameof(SubjectId))]
        public Subject Subject { get; set; } = null!;
    }
}
