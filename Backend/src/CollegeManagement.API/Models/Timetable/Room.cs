using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CollegeManagement.API.Models.Timetable
{
    [Table("Rooms")]
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [MaxLength(30)]
        public string RoomCode { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string RoomName { get; set; } = string.Empty;

        [Required]
        public int Capacity { get; set; } = 60;

        [Required]
        [MaxLength(50)]
        public string RoomType { get; set; } = "Classroom";

        [MaxLength(100)]
        public string? Building { get; set; }

        [MaxLength(50)]
        public string? Floor { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
