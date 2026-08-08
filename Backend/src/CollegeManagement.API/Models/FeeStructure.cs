using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models
{
    [Table("FeeStructures")]
    public class FeeStructure
    {
        [Key]
        [Column("FeeStructureId")]
        public int Id { get; set; }

        [NotMapped]
        public int FeeStructureId 
        { 
            get => Id; 
            set => Id = value; 
        }

        public int BoardId { get; set; }
        public int AcademicYearId { get; set; }
        public int GroupId { get; set; }

        public string FeeType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public AcademicYear AcademicYear { get; set; } = null!;

        public ICollection<FeeCollection> FeeCollections { get; set; } = new List<FeeCollection>();
        
        public Board Board { get; set; } = null!;
        public Group Group { get; set; } = null!;
    }
}