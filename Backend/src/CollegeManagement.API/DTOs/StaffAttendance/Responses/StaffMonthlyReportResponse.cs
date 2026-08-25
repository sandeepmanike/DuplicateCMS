using System.Collections.Generic;
using CollegeManagement.API.DTOs.Attendance.Responses;

namespace CollegeManagement.API.DTOs.StaffAttendance.Responses
{
    public class StaffMonthlyGridRowDto
    {
        public int FacultyId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

        /// <summary>
        /// Daily status array ("P", "A", "L", "LV", "H", "-").
        /// </summary>
        public List<string> DailyStatus { get; set; } = new List<string>();

        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public int LeaveCount { get; set; }
        public double Percentage { get; set; }
    }

    public class StaffMonthlyReportResponse
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string StaffTypeName { get; set; } = string.Empty;
        public int TotalWorkingDays { get; set; }
        public int TotalPresent { get; set; }
        public int TotalAbsent { get; set; }
        public int TotalLate { get; set; }
        public int TotalLeave { get; set; }
        public double OverallAttendancePercentage { get; set; }

        public List<DayHeaderDto> DayHeaders { get; set; } = new List<DayHeaderDto>();
        public List<StaffMonthlyGridRowDto> StaffRows { get; set; } = new List<StaffMonthlyGridRowDto>();
    }
}
