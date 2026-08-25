using System;

namespace CollegeManagement.API.DTOs.Examination.Responses
{
    public class AvailableHallDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string? BlockName { get; set; }
        public string? Floor { get; set; }
        public int Capacity { get; set; } = 60;
        public string RoomType { get; set; } = "Classroom";
        public bool IsAvailable { get; set; } = true;
    }
}
