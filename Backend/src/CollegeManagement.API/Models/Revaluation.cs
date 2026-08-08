using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    /// <summary>
    /// Represents a student's revaluation request.
    /// </summary>
    [Table("Revaluations")]
    public class Revaluation
    {
        /// <summary>
        /// Gets or sets the unique identifier of the revaluation request.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RevaluationId { get; set; }

        /// <summary>
        /// Gets or sets the result identifier.
        /// </summary>
        [Required]
        public int ResultId { get; set; }

        /// <summary>
        /// Gets or sets the student identifier.
        /// </summary>
        [Required]
        public int StudentId { get; set; }

        /// <summary>
        /// Gets or sets the reason for revaluation.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request date.
        /// </summary>
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the current status of the request.
        /// </summary>
        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the reviewed date.
        /// </summary>
        public DateTime? ReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets reviewer remarks.
        /// </summary>
        [MaxLength(500)]
        public string? Remarks { get; set; }

        /// <summary>
        /// Gets or sets the created date.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Gets or sets the updated date.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets the marks before revaluation.
        /// </summary>
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 1000)]
        public decimal OldMarks { get; set; }

        /// <summary>
        /// Gets or sets the marks after revaluation.
        /// </summary>
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 1000)]
        public decimal? NewMarks { get; set; }

        /// <summary>
        /// Gets or sets the reviewer identifier.
        /// </summary>
        public int? ReviewedBy { get; set; }

        /// <summary>
        /// Gets or sets whether the revaluation fee is paid.
        /// </summary>
        public bool FeePaid { get; set; }

        // Navigation Properties

        [ForeignKey(nameof(ResultId))]
        public virtual Result Result { get; set; } = null!;

        [ForeignKey(nameof(StudentId))]
        public virtual Student Student { get; set; } = null!;
    }
}