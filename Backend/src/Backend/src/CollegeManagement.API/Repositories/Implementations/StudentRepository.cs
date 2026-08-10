using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Students;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CollegeManagement.API.Repositories
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        // =====================================
        // GET ALL STUDENTS
        // =====================================

        public async Task<List<StudentListItemDto>> GetAllAsync()
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryAsync<StudentListItemDto>(
                "sp_GetAllStudents",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }

        // =====================================
        // GET STUDENT BY ID
        // =====================================

        public async Task<StudentResponse?> GetByIdAsync(int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<StudentResponse>(
                "sp_GetStudentById",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);
        }

        // =====================================
        // CREATE STUDENT
        // =====================================

        public async Task<StudentResponse> CreateAsync(CreateStudentRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryFirstOrDefaultAsync<StudentResponse>(
                "sp_CreateStudent",
                new
                {
                    // Admission
                    p_AdmissionNo = request.AdmissionNo,
                    p_RollNo = request.RollNo,
                    p_StudentName = request.StudentName,
                    p_Photo = request.Photo,

                    // Personal
                    p_Gender = request.Gender,
                    p_DateOfBirth = request.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                    p_BloodGroup = request.BloodGroup,
                    p_Email = request.Email,
                    p_MobileNumber = request.MobileNumber,
                    p_AadhaarNumber = request.AadhaarNumber,
                    p_Address = request.Address,

                    // Academic
                    p_Board = request.Board,
                    p_AcademicYearId = request.AcademicYearId,
                    p_AcademicLevel = request.AcademicLevel,
                    p_GroupId = request.GroupId,
                    p_Section = request.Section,
                    p_AdmissionDate = request.AdmissionDate.ToDateTime(TimeOnly.MinValue),
                    p_AdmissionType = request.AdmissionType,
                    p_Medium = request.Medium,
                    p_PreviousSchool = request.PreviousSchool,
                    p_PreviousHallTicketNumber = request.PreviousHallTicketNumber,
                    p_StudentCategory = request.StudentCategory,
                    p_ScholarshipStatus = request.ScholarshipStatus,

                    // Parent
                    p_FatherName = request.FatherName,
                    p_FatherMobile = request.FatherMobile,
                    p_MotherName = request.MotherName,
                    p_MotherMobile = request.MotherMobile,
                    p_GuardianName = request.GuardianName,
                    p_GuardianMobile = request.GuardianMobile,

                    // Fee
                    p_FeeAmount = request.FeeAmount,
                    p_FeePaid = request.FeePaid,
                    p_ScholarshipAmount = request.ScholarshipAmount,
                    p_FeeStatus = request.FeeStatus,

                    // Performance
                    p_AttendancePercentage = request.AttendancePercentage,
                    p_PerformanceGrade = request.PerformanceGrade,
                    p_CGPA = request.CGPA,
                    p_Rank = request.Rank,
                    p_Remarks = request.Remarks,

                    // Login
                    p_PasswordHash = request.PasswordHash,
                    p_IsFirstLogin = request.IsFirstLogin,

                    // Status
                    p_IsActive = request.IsActive
                },
                commandType: CommandType.StoredProcedure);

            return result!;
        }
        // =====================================
        // UPDATE STUDENT
        // =====================================

        public async Task<StudentResponse?> UpdateAsync(
            int studentId,
            UpdateStudentRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<StudentResponse>(
                "sp_UpdateStudent",
                new
                {
                    p_StudentId = studentId,

                    // Admission
                    p_AdmissionNo = request.AdmissionNo,
                    p_RollNo = request.RollNo,
                    p_StudentName = request.StudentName,
                    p_Photo = request.Photo,

                    // Personal
                    p_Gender = request.Gender,
                    p_DateOfBirth = request.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                    p_BloodGroup = request.BloodGroup,
                    p_Email = request.Email,
                    p_MobileNumber = request.MobileNumber,
                    p_AadhaarNumber = request.AadhaarNumber,
                    p_Address = request.Address,

                    // Academic
                    p_Board = request.Board,
                    p_AcademicYearId = request.AcademicYearId,
                    p_AcademicLevel = request.AcademicLevel,
                    p_GroupId = request.GroupId,
                    p_Section = request.Section,
                    p_AdmissionDate = request.AdmissionDate.ToDateTime(TimeOnly.MinValue),
                    p_AdmissionType = request.AdmissionType,
                    p_Medium = request.Medium,
                    p_PreviousSchool = request.PreviousSchool,
                    p_PreviousHallTicketNumber = request.PreviousHallTicketNumber,
                    p_StudentCategory = request.StudentCategory,
                    p_ScholarshipStatus = request.ScholarshipStatus,

                    // Parent
                    p_FatherName = request.FatherName,
                    p_FatherMobile = request.FatherMobile,
                    p_MotherName = request.MotherName,
                    p_MotherMobile = request.MotherMobile,
                    p_GuardianName = request.GuardianName,
                    p_GuardianMobile = request.GuardianMobile,

                    // Fee
                    p_FeeAmount = request.FeeAmount,
                    p_FeePaid = request.FeePaid,
                    p_ScholarshipAmount = request.ScholarshipAmount,
                    p_FeeStatus = request.FeeStatus,

                    // Performance
                    p_AttendancePercentage = request.AttendancePercentage,
                    p_PerformanceGrade = request.PerformanceGrade,
                    p_CGPA = request.CGPA,
                    p_Rank = request.Rank,
                    p_Remarks = request.Remarks,

                    // Login
                    p_PasswordHash = request.PasswordHash,
                    p_IsFirstLogin = request.IsFirstLogin,

                    // Status
                    p_IsActive = request.IsActive
                },
                commandType: CommandType.StoredProcedure);
        }

        // =====================================
        // DELETE STUDENT
        // =====================================

        public async Task<bool> DeleteAsync(int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryFirstAsync<int>(
                "sp_DeleteStudent",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);

            return result == 1;
        }

        // =====================================
        // GET STUDENT PROFILE
        // =====================================

        public async Task<StudentProfileDto?> GetProfileAsync(int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<StudentProfileDto>(
                "sp_GetStudentProfile",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);
        }

        // =====================================
        // UPDATE STUDENT PROFILE
        // =====================================

        public async Task<StudentProfileDto?> UpdateProfileAsync(
            int studentId,
            StudentProfileDto request)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<StudentProfileDto>(
                "sp_UpdateStudentProfile",
                new
                {
                    p_StudentId = studentId,
                    p_Photo = request.Photo,
                    p_Email = request.Email,
                    p_MobileNumber = request.MobileNumber,
                    p_Address = request.Address,
                    p_FatherName = request.FatherName,
                    p_FatherMobile = request.FatherMobile,
                    p_MotherName = request.MotherName,
                    p_MotherMobile = request.MotherMobile,
                    p_GuardianName = request.GuardianName,
                    p_GuardianMobile = request.GuardianMobile
                },
                commandType: CommandType.StoredProcedure);
        }
        // =====================================
        // CHANGE STUDENT SECTION
        // =====================================

        public async Task<bool> ChangeSectionAsync(
            int studentId,
            ChangeSectionRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_ChangeStudentSection",
                new
                {
                    p_StudentId = studentId,
                    p_Section = request.Section
                },
                commandType: CommandType.StoredProcedure);

            return true;
        }

        // =====================================
        // CHANGE STUDENT GROUP
        // =====================================

        public async Task<bool> ChangeGroupAsync(
            int studentId,
            ChangeGroupRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_ChangeStudentGroup",
                new
                {
                    p_StudentId = studentId,
                    p_GroupId = request.GroupId
                },
                commandType: CommandType.StoredProcedure);

            return true;
        }

        // =====================================
        // TRANSFER STUDENT
        // =====================================

        public async Task<bool> TransferAsync(
            int studentId,
            TransferStudentRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_TransferStudent",
                new
                {
                    p_StudentId = studentId,
                    p_Board = request.Board,
                    p_AcademicYearId = request.AcademicYearId,
                    p_AcademicLevel = request.AcademicLevel,
                    p_GroupId = request.GroupId,
                    p_Section = request.Section
                },
                commandType: CommandType.StoredProcedure);

            return true;
        }

        // =====================================
        // SUSPEND STUDENT
        // =====================================

        public async Task<bool> SuspendAsync(
            int studentId,
            SuspendStudentRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_SuspendStudent",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);

            return true;
        }

        // =====================================
        // ACTIVATE STUDENT
        // =====================================

        public async Task<bool> ActivateAsync(int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_ActivateStudent",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);

            return true;
        }

        // =====================================
        // RESET PASSWORD
        // =====================================

        public async Task<bool> ResetPasswordAsync(int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            await connection.ExecuteAsync(
                "sp_ResetStudentLogin",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);

            return true;
        }

        // =====================================
        // STUDENT DASHBOARD
        // =====================================

        public async Task<StudentDashboardDto?> GetDashboardAsync(int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<StudentDashboardDto>(
                "sp_GetStudentDashboard",
                new
                {
                    p_StudentId = studentId
                },
                commandType: CommandType.StoredProcedure);
        }
    }
}