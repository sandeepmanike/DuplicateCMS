using System;

namespace CollegeManagement.API.DTOs.Timetable
{
    public class CreateRoomDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;

        public string? RoomNumber
        {
            get => RoomCode;
            set
            {
                if (string.IsNullOrWhiteSpace(RoomCode) && !string.IsNullOrWhiteSpace(value))
                {
                    RoomCode = value;
                }
            }
        }

        public int Capacity { get; set; } = 60;
        public string RoomType { get; set; } = "Classroom";
        public string? BlockName { get; set; }

        public string? Building
        {
            get => BlockName;
            set
            {
                if (string.IsNullOrWhiteSpace(BlockName) && !string.IsNullOrWhiteSpace(value))
                {
                    BlockName = value;
                }
            }
        }

        public string? BuildingName
        {
            get => BlockName;
            set
            {
                if (string.IsNullOrWhiteSpace(BlockName) && !string.IsNullOrWhiteSpace(value))
                {
                    BlockName = value;
                }
            }
        }

        public string? Block
        {
            get => BlockName;
            set
            {
                if (string.IsNullOrWhiteSpace(BlockName) && !string.IsNullOrWhiteSpace(value))
                {
                    BlockName = value;
                }
            }
        }

        public string? Floor { get; set; }
        public bool IsActive { get; set; } = true;

        public string? Status
        {
            get => IsActive ? "Active" : "Inactive";
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    IsActive = value.Equals("Active", System.StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }

    public class UpdateRoomDto
    {
        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;

        public string? RoomNumber
        {
            get => RoomCode;
            set
            {
                if (string.IsNullOrWhiteSpace(RoomCode) && !string.IsNullOrWhiteSpace(value))
                {
                    RoomCode = value;
                }
            }
        }

        public int Capacity { get; set; } = 60;
        public string RoomType { get; set; } = "Classroom";
        public string? BlockName { get; set; }

        public string? Building
        {
            get => BlockName;
            set
            {
                if (string.IsNullOrWhiteSpace(BlockName) && !string.IsNullOrWhiteSpace(value))
                {
                    BlockName = value;
                }
            }
        }

        public string? BuildingName
        {
            get => BlockName;
            set
            {
                if (string.IsNullOrWhiteSpace(BlockName) && !string.IsNullOrWhiteSpace(value))
                {
                    BlockName = value;
                }
            }
        }

        public string? Block
        {
            get => BlockName;
            set
            {
                if (string.IsNullOrWhiteSpace(BlockName) && !string.IsNullOrWhiteSpace(value))
                {
                    BlockName = value;
                }
            }
        }

        public string? Floor { get; set; }
        public bool IsActive { get; set; } = true;

        public string? Status
        {
            get => IsActive ? "Active" : "Inactive";
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    IsActive = value.Equals("Active", System.StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }

    public class RoomResponseDto
    {
        public int RoomId { get; set; }
        public int Id => RoomId;

        public string RoomCode { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string RoomNumber => RoomCode;
        public int Capacity { get; set; }
        public string RoomType { get; set; } = string.Empty;
        public string? BlockName { get; set; }
        public string? Block => BlockName;
        public string? Building => BlockName;
        public string? BuildingName => BlockName;
        public string? Floor { get; set; }
        public bool IsActive { get; set; }
        public string Status => IsActive ? "Active" : "Inactive";

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class RoomFilterDto
    {
        public string? Building { get; set; }
        public string? BlockName
        {
            get => Building;
            set => Building = value;
        }
        public string? Block
        {
            get => Building;
            set => Building = value;
        }

        public string? Floor { get; set; }
        public string? RoomType { get; set; }
        public bool? IsActive { get; set; }

        public string? Status
        {
            get => IsActive.HasValue ? (IsActive.Value ? "Active" : "Inactive") : null;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    IsActive = value.Equals("Active", System.StringComparison.OrdinalIgnoreCase);
                }
            }
        }

        public string? SearchTerm { get; set; }
        public string? Search
        {
            get => SearchTerm;
            set => SearchTerm = value;
        }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
