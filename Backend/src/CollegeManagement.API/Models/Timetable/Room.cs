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
        [Column("RoomNumber")]
        [MaxLength(50)]
        public string RoomNumber { get; set; } = string.Empty;

        [NotMapped]
        public string RoomCode { get => RoomNumber; set => RoomNumber = value; }

        [NotMapped]
        public string RoomName { get => RoomNumber; set => RoomNumber = value; }

        [Column("BuildingName")]
        [MaxLength(100)]
        public string? BuildingName { get; set; }

        [NotMapped]
        public string? Building { get => BuildingName; set => BuildingName = value; }

        public int? Floor { get; set; }

        [Required]
        public int Capacity { get; set; } = 60;

        [Required]
        [MaxLength(50)]
        public string RoomType { get; set; } = "Classroom";

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
