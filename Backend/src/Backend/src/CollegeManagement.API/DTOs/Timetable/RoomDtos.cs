using System;

namespace CollegeManagement.API.DTOs.Timetable
{
    public class CreateRoomDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; } = 60;
        public string RoomType { get; set; } = "Classroom";
        public string? Building { get; set; }
        public string? Floor { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UpdateRoomDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; } = 60;
        public string RoomType { get; set; } = "Classroom";
        public string? Building { get; set; }
        public string? Floor { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class RoomResponseDto
    {
        public int RoomId { get; set; }
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public string? Building { get; set; }
        public string? Floor { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
