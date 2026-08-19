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

        // =========================================================
        // GET ALL STUDENTS
        // =========================================================

        public async Task<List<StudentListItemDto>> GetAllAsync()
        {
            var connection = _context.Database.GetDbConnection();

            var result = await connection.QueryAsync<StudentListItemDto>(
                "sp_GetAllStudents",
                commandType: CommandType.StoredProcedure);

            return result.ToList();
        }


        // =========================================================
        // GET STUDENT BY ID
        // =========================================================

        public async Task<StudentResponse?> GetByIdAsync(
            int studentId)
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


        // =========================================================
        // UPDATE STUDENT
        // =========================================================

        public async Task<StudentResponse?> UpdateAsync(
            int studentId,
            UpdateStudentRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<StudentResponse>(
                "sp_UpdateStudent",
                new
                {
                    // =================================================
                    // STUDENT
                    // =================================================

                    p_StudentId = studentId,

                    p_AdmissionNo = request.AdmissionNo,
                    p_RollNo = request.RollNo,
                    p_StudentName = request.StudentName,
                    p_Photo = request.Photo,

                    // =================================================
                    // PERSONAL
                    // =================================================

                    p_Gender = request.Gender,
                    p_DateOfBirth = request.DateOfBirth,
                    p_BloodGroup = request.BloodGroup,
                    p_Email = request.Email,
                    p_MobileNumber = request.MobileNumber,
                    p_AadhaarNumber = request.AadhaarNumber,
                    p_Nationality = request.Nationality,
                    p_Religion = request.Religion,
                    p_Category = request.Category,

                    // =================================================
                    // ADDRESS
                    // =================================================

                    p_Address = request.Address,
                    p_City = request.City,
                    p_District = request.District,
                    p_State = request.State,
                    p_Pincode = request.Pincode,

                    // =================================================
                    // ACADEMIC IDS
                    // =================================================

                    p_BoardId = request.BoardId,
                    p_AcademicYearId = request.AcademicYearId,
                    p_AcademicLevelId =
                        request.AcademicLevelId,
                    p_GroupId = request.GroupId,
                    p_SectionId = request.SectionId,

                    // =================================================
                    // ADMISSION
                    // =================================================

                    p_AdmissionDate =
                        request.AdmissionDate,

                    p_AdmissionType =
                        request.AdmissionType,

                    p_AdmissionQuota =
                        request.AdmissionQuota,

                    p_Medium =
                        request.Medium,

                    p_SecondLanguage =
                        request.SecondLanguage,

                    // =================================================
                    // PREVIOUS EDUCATION
                    // =================================================

                    p_PreviousSchool =
                        request.PreviousSchool,

                    p_PreviousHallTicketNumber =
                        request.PreviousHallTicketNumber,

                    p_PreviousBoard =
                        request.PreviousBoard,

                    p_PreviousYearOfPassing =
                        request.PreviousYearOfPassing,

                    p_PreviousPercentage =
                        request.PreviousPercentage,

                    // =================================================
                    // SCHOLARSHIP
                    // =================================================

                    p_StudentCategory =
                        request.StudentCategory,

                    p_ScholarshipStatus =
                        request.ScholarshipStatus,

                    p_ScholarshipAmount =
                        request.ScholarshipAmount,

                    // =================================================
                    // PARENT
                    // =================================================

                    p_FatherName =
                        request.FatherName,

                    p_FatherOccupation =
                        request.FatherOccupation,

                    p_FatherMobile =
                        request.FatherMobile,

                    p_FatherEmail =
                        request.FatherEmail,

                    p_MotherName =
                        request.MotherName,

                    p_MotherOccupation =
                        request.MotherOccupation,

                    p_MotherMobile =
                        request.MotherMobile,

                    p_MotherEmail =
                        request.MotherEmail,

                    p_GuardianName =
                        request.GuardianName,

                    p_GuardianMobile =
                        request.GuardianMobile,

                    p_GuardianEmail =
                        request.GuardianEmail,

                    p_AnnualIncome =
                        request.AnnualIncome,

                    // =================================================
                    // FEES
                    // =================================================

                    p_FeeAmount =
                        request.FeeAmount,

                    p_FeePaid =
                        request.FeePaid,

                    p_FeeStatus =
                        request.FeeStatus,

                    // =================================================
                    // PERFORMANCE
                    // =================================================

                    p_AttendancePercentage =
                        request.AttendancePercentage,

                    p_PerformanceGrade =
                        request.PerformanceGrade,

                    p_CGPA =
                        request.CGPA,

                    p_Rank =
                        request.Rank,

                    // =================================================
                    // DOCUMENTS
                    // =================================================

                    p_BirthCertificate =
                        request.BirthCertificate,

                    p_TransferCertificate =
                        request.TransferCertificate,

                    p_StudyCertificate =
                        request.StudyCertificate,

                    p_AadhaarDocument =
                        request.AadhaarDocument,

                    p_CommunityCertificate =
                        request.CommunityCertificate,

                    p_IncomeCertificate =
                        request.IncomeCertificate,

                    p_CasteCertificate =
                        request.CasteCertificate,

                    p_TenthCertificate =
                        request.TenthCertificate,

                    p_MarksMemo =
                        request.MarksMemo,

                    // =================================================
                    // REMARKS
                    // =================================================

                    p_Remarks =
                        request.Remarks,

                    // =================================================
                    // LOGIN / STATUS
                    // =================================================

                    p_PasswordHash =
                        (string?)null,

                    p_IsFirstLogin =
                        (bool?)null,

                    p_IsActive =
                        request.IsActive
                },
                commandType: CommandType.StoredProcedure);
        }


        // =========================================================
        // DELETE STUDENT
        // =========================================================

        public async Task<bool> DeleteAsync(
            int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_DeleteStudent",
                    new
                    {
                        p_StudentId = studentId
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }


        // =========================================================
        // GET STUDENT PROFILE
        // =========================================================

        public async Task<StudentProfileDto?> GetProfileAsync(
            int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection
                .QueryFirstOrDefaultAsync<StudentProfileDto>(
                    "sp_GetStudentProfile",
                    new
                    {
                        p_StudentId = studentId
                    },
                    commandType: CommandType.StoredProcedure);
        }


        // =========================================================
        // UPDATE STUDENT PROFILE
        // =========================================================

        public async Task<StudentProfileDto?> UpdateProfileAsync(
            int studentId,
            UpdateStudentProfileRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection
                .QueryFirstOrDefaultAsync<StudentProfileDto>(
                    "sp_UpdateStudentProfile",
                    new
                    {
                        p_StudentId = studentId,

                        // Personal
                        p_StudentName =
                            request.StudentName,

                        p_Photo =
                            request.Photo,

                        p_Gender =
                            request.Gender,

                        p_DateOfBirth =
                            request.DateOfBirth,

                        p_BloodGroup =
                            request.BloodGroup,

                        p_Email =
                            request.Email,

                        p_MobileNumber =
                            request.MobileNumber,

                        p_AadhaarNumber =
                            request.AadhaarNumber,

                        // Address
                        p_Address =
                            request.Address,
                        p_City =
                            request.City,

                        p_District =
                            request.District,

                        p_State =
                            request.State,

                        p_Pincode =
                            request.Pincode,

                        p_Nationality =
                            request.Nationality,

                        p_Religion =
                            request.Religion,

                        p_Category =
                            request.Category,

                        // Father
                        p_FatherName =
                            request.FatherName,

                        p_FatherOccupation =
                            request.FatherOccupation,

                        p_FatherMobile =
                            request.FatherMobile,

                        p_FatherEmail =
                            request.FatherEmail,

                        // Mother
                        p_MotherName =
                            request.MotherName,

                        p_MotherOccupation =
                            request.MotherOccupation,

                        p_MotherMobile =
                            request.MotherMobile,

                        p_MotherEmail =
                            request.MotherEmail,

                        // Guardian
                        p_GuardianName =
                            request.GuardianName,

                        p_GuardianMobile =
                            request.GuardianMobile,

                        p_GuardianEmail =
                            request.GuardianEmail,

                        // Remarks
                        p_Remarks =
                            request.Remarks
                    },
                    commandType: CommandType.StoredProcedure);
        }


        // =========================================================
        // CHANGE SECTION
        // =========================================================

        public async Task<bool> ChangeSectionAsync(
            int studentId,
            ChangeSectionRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ChangeStudentSection",
                    new
                    {
                        p_StudentId = studentId,
                        p_SectionId = request.SectionId
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }


        // =========================================================
        // CHANGE GROUP
        // =========================================================

        public async Task<bool> ChangeGroupAsync(
            int studentId,
            ChangeGroupRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ChangeStudentGroup",
                    new
                    {
                        p_StudentId = studentId,
                        p_GroupId = request.GroupId
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }


        // =========================================================
        // TRANSFER STUDENT
        // =========================================================

        public async Task<bool> TransferAsync(
            int studentId,
            TransferStudentRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_TransferStudent",
                    new
                    {
                        p_StudentId = studentId,

                        p_BoardId =
                            request.BoardId,

                        p_AcademicYearId =
                            request.AcademicYearId,

                        p_AcademicLevelId =
                            request.AcademicLevelId,

                        p_GroupId =
                            request.GroupId,

                        p_SectionId =
                            request.SectionId
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }


        // =========================================================
        // SUSPEND STUDENT
        // =========================================================

        public async Task<bool> SuspendAsync(
            int studentId,
            SuspendStudentRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_SuspendStudent",
                    new
                    {
                        p_StudentId = studentId,
                        p_Reason = request.Reason
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }


        // =========================================================
        // ACTIVATE STUDENT
        // =========================================================

        public async Task<bool> ActivateAsync(
            int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ActivateStudent",
                    new
                    {
                        p_StudentId = studentId
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }


        // =========================================================
        // RESET PASSWORD
        // =========================================================

        public async Task<bool> ResetPasswordAsync(
            int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_ResetStudentLogin",
                    new
                    {
                        p_StudentId = studentId
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }


        // =========================================================
        // DASHBOARD
        // =========================================================

        public async Task<StudentDashboardDto?> GetDashboardAsync(
            int studentId)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection
                .QueryFirstOrDefaultAsync<StudentDashboardDto>(
                    "sp_GetStudentDashboard",
                    new
                    {
                        p_StudentId = studentId
                    },
                    commandType: CommandType.StoredProcedure);
        }


        // =========================================================
        // SEARCH
        // =========================================================

        public async Task<List<StudentListItemDto>> SearchAsync(
            string? search,
            int? groupId,
            int? sectionId,
            int? academicYearId,
            bool? isActive)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryAsync<StudentListItemDto>(
                    "sp_SearchStudents",
                    new
                    {
                        p_Search = search,
                        p_GroupId = groupId,
                        p_SectionId = sectionId,
                        p_AcademicYearId = academicYearId,
                        p_IsActive = isActive
                    },
                    commandType: CommandType.StoredProcedure);

            return result.ToList();
        }


        // =========================================================
        // GET BY GROUP
        // =========================================================

        public async Task<List<StudentListItemDto>> GetByGroupAsync(
            int groupId)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryAsync<StudentListItemDto>(
                    "sp_GetStudentsByGroup",
                    new
                    {
                        p_GroupId = groupId
                    },
                    commandType: CommandType.StoredProcedure);

            return result.ToList();
        }


        // =========================================================
        // GET BY SECTION
        // =========================================================

        public async Task<List<StudentListItemDto>> GetBySectionAsync(
            int sectionId)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryAsync<StudentListItemDto>(
                    "sp_GetStudentsBySection",
                    new
                    {
                        p_SectionId = sectionId
                    },
                    commandType: CommandType.StoredProcedure);

            return result.ToList();
        }


        // =========================================================
        // GET ACTIVE STUDENTS
        // =========================================================

        public async Task<List<StudentListItemDto>> GetActiveAsync()
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryAsync<StudentListItemDto>(
                    "sp_GetActiveStudents",
                    commandType: CommandType.StoredProcedure);

            return result.ToList();
        }


        // =========================================================
        // EMAIL EXISTS
        // =========================================================

        public async Task<bool> EmailExistsAsync(
            string email,
            int? excludeStudentId = null)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CheckStudentEmail",
                    new
                    {
                        p_Email = email,
                        p_ExcludeStudentId =
                            excludeStudentId
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }


        // =========================================================
        // MOBILE EXISTS
        // =========================================================

        public async Task<bool> MobileExistsAsync(
            string mobile,
            int? excludeStudentId = null)
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_CheckStudentMobile",
                    new
                    {
                        p_MobileNumber = mobile,
                        p_ExcludeStudentId =
                            excludeStudentId
                    },
                    commandType: CommandType.StoredProcedure);

            return result == 1;
        }
    }
}