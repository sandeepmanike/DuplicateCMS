using System.Data;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Students;
using CollegeManagement.API.Exceptions;
using CollegeManagement.API.Repositories;
using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _context;

        public StudentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<StudentListItemDto>> GetAllAsync()
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<StudentListItemDto>(
                "sp_GetAllStudents",
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<StudentResponse?> GetByIdAsync(int studentId)
        {
            var connection = _context.Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<StudentResponse>(
                "sp_GetStudentById",
                new { p_StudentId = studentId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<StudentProfileDto?> GetProfileAsync(int studentId)
        {
            var connection = _context.Database.GetDbConnection();
            return await connection.QueryFirstOrDefaultAsync<StudentProfileDto>(
                "sp_GetStudentProfile",
                new { p_StudentId = studentId },
                commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(CreateStudentRequest request)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                return await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CreateStudent",
                    new
                    {
                        p_AdmissionNo = request.AdmissionNo,
                        p_RollNo = request.RollNo,
                        p_StudentName = request.StudentName,
                        p_Photo = request.Photo,
                        p_Gender = request.Gender,
                        p_DateOfBirth = request.DateOfBirth,
                        p_BloodGroup = request.BloodGroup,
                        p_Email = request.Email,
                        p_MobileNumber = request.MobileNumber,
                        p_AadhaarNumber = request.AadhaarNumber,
                        p_Nationality = request.Nationality,
                        p_Religion = request.Religion,
                        p_Category = request.Category,
                        p_Address = request.Address,
                        p_City = request.City,
                        p_District = request.District,
                        p_State = request.State,
                        p_Pincode = request.Pincode,
                        p_BoardId = request.BoardId,
                        p_AcademicYearId = request.AcademicYearId,
                        p_AcademicLevelId = request.AcademicLevelId,
                        p_GroupId = request.GroupId,
                        p_SectionId = request.SectionId,
                        p_AdmissionDate = request.AdmissionDate,
                        p_AdmissionType = request.AdmissionType,
                        p_AdmissionQuota = request.AdmissionQuota,
                        p_Medium = request.Medium,
                        p_SecondLanguage = request.SecondLanguage,
                        p_PreviousSchool = request.PreviousSchool,
                        p_PreviousHallTicketNumber = request.PreviousHallTicketNumber,
                        p_PreviousBoard = request.PreviousBoard,
                        p_PreviousYearOfPassing = request.PreviousYearOfPassing,
                        p_PreviousPercentage = request.PreviousPercentage,
                        p_StudentCategory = request.StudentCategory,
                        p_ScholarshipStatus = request.ScholarshipStatus,
                        p_ScholarshipAmount = request.ScholarshipAmount,
                        p_FatherName = request.FatherName,
                        p_FatherOccupation = request.FatherOccupation,
                        p_FatherMobile = request.FatherMobile,
                        p_FatherEmail = request.FatherEmail,
                        p_MotherName = request.MotherName,
                        p_MotherOccupation = request.MotherOccupation,
                        p_MotherMobile = request.MotherMobile,
                        p_MotherEmail = request.MotherEmail,
                        p_GuardianName = request.GuardianName,
                        p_GuardianMobile = request.GuardianMobile,
                        p_GuardianEmail = request.GuardianEmail,
                        p_AnnualIncome = request.AnnualIncome,
                        p_FeeAmount = request.FeeAmount,
                        p_FeePaid = request.FeePaid,
                        p_FeeStatus = request.FeeStatus,
                        p_AttendancePercentage = request.AttendancePercentage,
                        p_PerformanceGrade = request.PerformanceGrade,
                        p_CGPA = request.CGPA,
                        p_Rank = request.Rank,
                        p_BirthCertificate = request.BirthCertificate,
                        p_TransferCertificate = request.TransferCertificate,
                        p_StudyCertificate = request.StudyCertificate,
                        p_AadhaarDocument = request.AadhaarDocument,
                        p_CommunityCertificate = request.CommunityCertificate,
                        p_IncomeCertificate = request.IncomeCertificate,
                        p_CasteCertificate = request.CasteCertificate,
                        p_TenthCertificate = request.TenthCertificate,
                        p_MarksMemo = request.MarksMemo,
                        p_Remarks = request.Remarks,
                        p_PasswordHash = (string?)null,
                        p_IsFirstLogin = (bool?)null,
                        p_IsActive = true
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new ConflictException("A student with this admission number, roll number, or email already exists.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<StudentResponse?> UpdateAsync(int studentId, UpdateStudentRequest request)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                return await connection.QueryFirstOrDefaultAsync<StudentResponse>(
                    "sp_UpdateStudent",
                    new
                    {
                        p_StudentId = studentId,
                        p_AdmissionNo = request.AdmissionNo,
                        p_RollNo = request.RollNo,
                        p_StudentName = request.StudentName,
                        p_Photo = request.Photo,
                        p_Gender = request.Gender,
                        p_DateOfBirth = request.DateOfBirth,
                        p_BloodGroup = request.BloodGroup,
                        p_Email = request.Email,
                        p_MobileNumber = request.MobileNumber,
                        p_AadhaarNumber = request.AadhaarNumber,
                        p_Nationality = request.Nationality,
                        p_Religion = request.Religion,
                        p_Category = request.Category,
                        p_Address = request.Address,
                        p_City = request.City,
                        p_District = request.District,
                        p_State = request.State,
                        p_Pincode = request.Pincode,
                        p_BoardId = request.BoardId,
                        p_AcademicYearId = request.AcademicYearId,
                        p_AcademicLevelId = request.AcademicLevelId,
                        p_GroupId = request.GroupId,
                        p_SectionId = request.SectionId,
                        p_AdmissionDate = request.AdmissionDate,
                        p_AdmissionType = request.AdmissionType,
                        p_AdmissionQuota = request.AdmissionQuota,
                        p_Medium = request.Medium,
                        p_SecondLanguage = request.SecondLanguage,
                        p_PreviousSchool = request.PreviousSchool,
                        p_PreviousHallTicketNumber = request.PreviousHallTicketNumber,
                        p_PreviousBoard = request.PreviousBoard,
                        p_PreviousYearOfPassing = request.PreviousYearOfPassing,
                        p_PreviousPercentage = request.PreviousPercentage,
                        p_StudentCategory = request.StudentCategory,
                        p_ScholarshipStatus = request.ScholarshipStatus,
                        p_ScholarshipAmount = request.ScholarshipAmount,
                        p_FatherName = request.FatherName,
                        p_FatherOccupation = request.FatherOccupation,
                        p_FatherMobile = request.FatherMobile,
                        p_FatherEmail = request.FatherEmail,
                        p_MotherName = request.MotherName,
                        p_MotherOccupation = request.MotherOccupation,
                        p_MotherMobile = request.MotherMobile,
                        p_MotherEmail = request.MotherEmail,
                        p_GuardianName = request.GuardianName,
                        p_GuardianMobile = request.GuardianMobile,
                        p_GuardianEmail = request.GuardianEmail,
                        p_AnnualIncome = request.AnnualIncome,
                        p_FeeAmount = request.FeeAmount,
                        p_FeePaid = request.FeePaid,
                        p_FeeStatus = request.FeeStatus,
                        p_AttendancePercentage = request.AttendancePercentage,
                        p_PerformanceGrade = request.PerformanceGrade,
                        p_CGPA = request.CGPA,
                        p_Rank = request.Rank,
                        p_BirthCertificate = request.BirthCertificate,
                        p_TransferCertificate = request.TransferCertificate,
                        p_StudyCertificate = request.StudyCertificate,
                        p_AadhaarDocument = request.AadhaarDocument,
                        p_CommunityCertificate = request.CommunityCertificate,
                        p_IncomeCertificate = request.IncomeCertificate,
                        p_CasteCertificate = request.CasteCertificate,
                        p_TenthCertificate = request.TenthCertificate,
                        p_MarksMemo = request.MarksMemo,
                        p_Remarks = request.Remarks,
                        p_PasswordHash = (string?)null,
                        p_IsFirstLogin = (bool?)null,
                        p_IsActive = request.IsActive
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new ConflictException("A student with this admission number, roll number, or email already exists.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<bool> DeleteAsync(int studentId)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_DeleteStudent",
                    new { p_StudentId = studentId },
                    commandType: CommandType.StoredProcedure);
                return result == 1;
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<StudentProfileDto?> UpdateProfileAsync(int studentId, UpdateStudentProfileRequest request)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                return await connection.QueryFirstOrDefaultAsync<StudentProfileDto>(
                    "sp_UpdateStudentProfile",
                    new
                    {
                        p_StudentId = studentId,
                        p_StudentName = request.StudentName,
                        p_Photo = request.Photo,
                        p_Gender = request.Gender,
                        p_DateOfBirth = request.DateOfBirth.ToDateTime(TimeOnly.MinValue),
                        p_BloodGroup = request.BloodGroup,
                        p_Email = request.Email,
                        p_MobileNumber = request.MobileNumber,
                        p_AadhaarNumber = request.AadhaarNumber,
                        p_Address = request.Address,
                        p_City = request.City,
                        p_District = request.District,
                        p_State = request.State,
                        p_Pincode = request.Pincode,
                        p_Nationality = request.Nationality,
                        p_Religion = request.Religion,
                        p_Category = request.Category,
                        p_FatherName = request.FatherName,
                        p_FatherOccupation = request.FatherOccupation,
                        p_FatherMobile = request.FatherMobile,
                        p_FatherEmail = request.FatherEmail,
                        p_MotherName = request.MotherName,
                        p_MotherOccupation = request.MotherOccupation,
                        p_MotherMobile = request.MotherMobile,
                        p_MotherEmail = request.MotherEmail,
                        p_GuardianName = request.GuardianName,
                        p_GuardianMobile = request.GuardianMobile,
                        p_GuardianEmail = request.GuardianEmail,
                        p_Remarks = request.Remarks
                    },
                    commandType: CommandType.StoredProcedure);
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.Number == 1062)
            {
                throw new ConflictException("A student with this email address already exists.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<bool> ChangeSectionAsync(int studentId, ChangeSectionRequest request)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ChangeStudentSection",
                    new
                    {
                        p_StudentId = studentId,
                        p_SectionId = request.SectionId
                    },
                    commandType: CommandType.StoredProcedure);
                return result == 1;
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<bool> ChangeGroupAsync(int studentId, ChangeGroupRequest request)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ChangeStudentGroup",
                    new
                    {
                        p_StudentId = studentId,
                        p_GroupId = request.GroupId,
                        p_SectionId = request.SectionId
                    },
                    commandType: CommandType.StoredProcedure);
                return result == 1;
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<bool> TransferAsync(int studentId, TransferStudentRequest request)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_TransferStudent",
                    new
                    {
                        p_StudentId = studentId,
                        p_BoardId = request.BoardId,
                        p_AcademicYearId = request.AcademicYearId,
                        p_AcademicLevelId = request.AcademicLevelId,
                        p_GroupId = request.GroupId,
                        p_SectionId = request.SectionId,
                        p_Remarks = request.Remarks
                    },
                    commandType: CommandType.StoredProcedure);
                return result == 1;
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<bool> SuspendAsync(int studentId, SuspendStudentRequest request)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_SuspendStudent",
                    new
                    {
                        p_StudentId = studentId,
                        p_Reason = request.Reason
                    },
                    commandType: CommandType.StoredProcedure);
                return result == 1;
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<bool> ActivateAsync(int studentId)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ActivateStudent",
                    new
                    {
                        p_StudentId = studentId
                    },
                    commandType: CommandType.StoredProcedure);
                return result == 1;
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

        public async Task<bool> ResetPasswordAsync(int studentId)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ResetStudentLogin",
                    new
                    {
                        p_StudentId = studentId
                    },
                    commandType: CommandType.StoredProcedure);
                return result == 1;
            }
            catch (MySqlException ex) when (ex.Message.Contains("Student not found"))
            {
                throw new NotFoundException($"Student with ID {studentId} not found.");
            }
            catch (MySqlException ex) when (ex.SqlState == "45000" || ex.Number == 1644)
            {
                throw new ValidationException(ex.Message);
            }
        }

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

        public async Task<List<StudentListItemDto>> SearchAsync(
            string? search,
            int? boardId,
            int? academicYearId,
            int? academicLevelId,
            int? groupId,
            int? sectionId,
            bool? isActive)
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<StudentListItemDto>(
                "sp_SearchStudents",
                new
                {
                    p_Search = search,
                    p_BoardId = boardId,
                    p_AcademicYearId = academicYearId,
                    p_AcademicLevelId = academicLevelId,
                    p_GroupId = groupId,
                    p_SectionId = sectionId,
                    p_IsActive = isActive
                },
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<StudentListItemDto>> GetByGroupAsync(int groupId)
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<StudentListItemDto>(
                "sp_GetStudentsByGroupId",
                new { p_GroupId = groupId },
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<StudentListItemDto>> GetBySectionAsync(int sectionId)
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<StudentListItemDto>(
                "sp_GetStudentsBySectionId",
                new { p_SectionId = sectionId },
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<List<StudentListItemDto>> GetActiveAsync()
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<StudentListItemDto>(
                "sp_GetActiveStudents",
                commandType: CommandType.StoredProcedure);
            return result.ToList();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeStudentId = null)
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "sp_CheckStudentEmail",
                new
                {
                    p_Email = email,
                    p_ExcludeStudentId = excludeStudentId
                },
                commandType: CommandType.StoredProcedure);
            return result == 1;
        }

        public async Task<bool> MobileExistsAsync(string mobile, int? excludeStudentId = null)
        {
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "sp_CheckStudentMobile",
                new
                {
                    p_MobileNumber = mobile,
                    p_ExcludeStudentId = excludeStudentId
                },
                commandType: CommandType.StoredProcedure);
            return result == 1;
        }
    }
}
