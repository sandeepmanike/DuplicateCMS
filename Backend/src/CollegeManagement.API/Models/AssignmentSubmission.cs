using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("AssignmentSubmissions")]
    public class AssignmentSubmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignmentSubmissionId { get; set; }

        [Required]
        public int AssignmentId { get; set; }

        [Required]
        [MaxLength(100)]
        public string StudentName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string SubmissionFile { get; set; } = string.Empty;

        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}