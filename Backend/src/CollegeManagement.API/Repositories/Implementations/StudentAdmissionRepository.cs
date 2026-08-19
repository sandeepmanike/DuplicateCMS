using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.StudentAdmission;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class StudentAdmissionRepository : IStudentAdmissionRepository
    {
        private readonly AppDbContext _context;

        public StudentAdmissionRepository(AppDbContext context)
        {
            _context = context;
        }


        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<IEnumerable<StudentAdmissionResponseDto>> GetAllAsync()
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryAsync<StudentAdmissionResponseDto>(
                    "sp_GetAllStudentAdmissions",
                    commandType: CommandType.StoredProcedure);

            return result;
        }


        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<StudentAdmissionResponseDto?> GetByIdAsync(
            int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<StudentAdmissionResponseDto>(
                "sp_GetStudentAdmissionById",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);
        }


        // =========================================================
        // CREATE
        // =========================================================

        public async Task<StudentAdmissionResponseDto> CreateAsync(
            CreateStudentAdmissionRequest request,
            string? studentPhoto,
            string? birthCertificate,
            string? transferCertificate,
            string? studyCertificate,
            string? aadhaarDocument,
            string? communityCertificate,
            string? incomeCertificate,
            string? casteCertificate,
            string? tenthCertificate,
            string? marksMemo)
        {
            var connection = _context.Database.GetDbConnection();
            // Generate admission number only once during CREATE
            var admissionNumber = await GenerateAdmissionNumberAsync();

            var result =
    await connection.QueryFirstOrDefaultAsync<StudentAdmissionResponseDto>(
        "sp_CreateAdmission",
        new
        {
            p_AdmissionNo = request.AdmissionNo,
            p_AdmissionDate = request.AdmissionDate,
            p_AdmissionQuota = request.AdmissionQuota,

            p_FirstName = request.FirstName,
            p_LastName = request.LastName,
            p_Gender = request.Gender,
            p_DateOfBirth = request.DateOfBirth,
            p_BloodGroup = request.BloodGroup,
            p_Email = request.Email,
            p_MobileNumber = request.MobileNumber,
            p_StudentPhoto = studentPhoto,
            p_AadhaarNumber = request.AadhaarNumber,
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

            p_AnnualIncome = request.AnnualIncome,

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
            p_Medium = request.Medium,
            p_SecondLanguage = request.SecondLanguage,
            p_AdmissionType = request.AdmissionType,

            p_PreviousSchool = request.PreviousSchool,
            p_PreviousYearOfPassing = request.PreviousYearOfPassing,
            p_PreviousBoard = request.PreviousBoard,
            p_PreviousPercentage = request.PreviousPercentage,

            p_ScholarshipStatus = request.ScholarshipStatus,

            p_BirthCertificate = birthCertificate,
            p_TransferCertificate = transferCertificate,
            p_StudyCertificate = studyCertificate,
            p_AadhaarDocument = aadhaarDocument,
            p_CommunityCertificate = communityCertificate,
            p_IncomeCertificate = incomeCertificate,
            p_CasteCertificate = casteCertificate,
            p_MarksMemo = marksMemo,


            p_Remarks = request.Remarks
        },
        commandType: CommandType.StoredProcedure);
            if (result == null)
            {
                throw new Exception(
                    "Admission was created but no data was returned.");
            }

            return result;
        }


        // =========================================================
        // UPDATE
        // =========================================================

        public async Task<StudentAdmissionResponseDto?> UpdateAsync(
            int admissionId,
            UpdateStudentAdmissionRequest request,
            string? studentPhoto,
            string? birthCertificate,
            string? transferCertificate,
            string? studyCertificate,
            string? aadhaarDocument,
            string? communityCertificate,
            string? incomeCertificate,
            string? casteCertificate,
            string? tenthCertificate,
            string? marksMemo)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<StudentAdmissionResponseDto>(
                "sp_UpdateStudentAdmission",
                new
                {
                    // -------------------------------------------------
                    // ADMISSION
                    // -------------------------------------------------

                    p_AdmissionId = admissionId,
                    p_AdmissionDate = request.AdmissionDate,
                    p_AdmissionQuota = request.AdmissionQuota,


                    // -------------------------------------------------
                    // STUDENT
                    // -------------------------------------------------

                    p_FirstName = request.FirstName,
                    p_LastName = request.LastName,
                    p_Gender = request.Gender,
                    p_DateOfBirth = request.DateOfBirth,
                    p_BloodGroup = request.BloodGroup,
                    p_Email = request.Email,
                    p_MobileNumber = request.MobileNumber,

                    // NULL means keep existing file.
                    p_StudentPhoto = studentPhoto,

                    p_AadhaarNumber = request.AadhaarNumber,
                    p_Nationality = request.Nationality,
                    p_Religion = request.Religion,
                    p_Category = request.Category,


                    // -------------------------------------------------
                    // FATHER
                    // -------------------------------------------------

                    p_FatherName = request.FatherName,
                    p_FatherOccupation = request.FatherOccupation,
                    p_FatherMobile = request.FatherMobile,
                    p_FatherEmail = request.FatherEmail,


                    // -------------------------------------------------
                    // MOTHER
                    // -------------------------------------------------

                    p_MotherName = request.MotherName,
                    p_MotherOccupation = request.MotherOccupation,
                    p_MotherMobile = request.MotherMobile,
                    p_MotherEmail = request.MotherEmail,


                    // -------------------------------------------------
                    // GUARDIAN
                    // -------------------------------------------------

                    p_GuardianName = request.GuardianName,
                    p_GuardianMobile = request.GuardianMobile,
                    p_GuardianEmail = request.GuardianEmail,


                    // -------------------------------------------------
                    // INCOME
                    // -------------------------------------------------

                    p_AnnualIncome = request.AnnualIncome,


                    // -------------------------------------------------
                    // ADDRESS
                    // -------------------------------------------------

                    p_Address = request.Address,
                    p_City = request.City,
                    p_District = request.District,
                    p_State = request.State,
                    p_Pincode = request.Pincode,


                    // -------------------------------------------------
                    // ACADEMIC
                    // -------------------------------------------------

                    p_BoardId = request.BoardId,
                    p_AcademicYearId = request.AcademicYearId,
                    p_AcademicLevelId = request.AcademicLevelId,
                    p_GroupId = request.GroupId,
                    p_SectionId = request.SectionId,
                    p_Medium = request.Medium,
                    p_SecondLanguage = request.SecondLanguage,
                    p_AdmissionType = request.AdmissionType,


                    // -------------------------------------------------
                    // PREVIOUS EDUCATION
                    // -------------------------------------------------

                    p_PreviousSchool = request.PreviousSchool,
                    p_PreviousYearOfPassing = request.PreviousYearOfPassing,
                    p_PreviousBoard = request.PreviousBoard,
                    p_PreviousPercentage = request.PreviousPercentage,


                    // -------------------------------------------------
                    // SCHOLARSHIP
                    // -------------------------------------------------

                    p_ScholarshipStatus = request.ScholarshipStatus,


                    // -------------------------------------------------
                    // DOCUMENTS
                    // -------------------------------------------------

                    p_BirthCertificate = birthCertificate,
                    p_TransferCertificate = transferCertificate,
                    p_StudyCertificate = studyCertificate,
                    p_AadhaarDocument = aadhaarDocument,
                    p_CommunityCertificate = communityCertificate,
                    p_IncomeCertificate = incomeCertificate,
                    p_CasteCertificate = casteCertificate,
                    p_TenthCertificate = tenthCertificate,
                    p_MarksMemo = marksMemo,


                    // -------------------------------------------------
                    // REMARKS
                    // -------------------------------------------------

                    p_Remarks = request.Remarks
                },
                commandType: CommandType.StoredProcedure);
        }


        // =========================================================
        // DELETE / SOFT DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            var affected =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_DeleteStudentAdmission",
                    new
                    {
                        p_AdmissionId = admissionId
                    },
                    commandType: CommandType.StoredProcedure);

            return affected > 0;
        }
        //validation//
        // =========================================================
        // SUBMIT
        // =========================================================

        public async Task<dynamic?> SubmitAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync(
                "sp_SubmitStudentAdmission",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);
        }


        // =========================================================
        // VERIFY
        // =========================================================

        public async Task<bool> VerifyAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            var affected =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_VerifyStudentAdmission",
                    new
                    {
                        p_AdmissionId = admissionId
                    },
                    commandType: CommandType.StoredProcedure);

            return affected > 0;
        }


        // =========================================================
        // APPROVE
        // Creates Student + generates RollNo
        // =========================================================

        public async Task<AdmissionApprovalResponseDto?> ApproveAsync(
            int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            return await connection.QueryFirstOrDefaultAsync<AdmissionApprovalResponseDto>(
                "sp_ApproveStudentAdmission",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);
        }


        // =========================================================
        // REJECT
        // =========================================================

        public async Task<bool> RejectAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            var affected =
                await connection.QueryFirstOrDefaultAsync<int>(
                    "sp_RejectStudentAdmission",
                    new
                    {
                        p_AdmissionId = admissionId
                    },
                    commandType: CommandType.StoredProcedure);

            return affected > 0;
        }

        // =========================================================
        // GENERATE ADMISSION NUMBER
        // =========================================================

        public async Task<string> GenerateAdmissionNumberAsync()
        {
            var connection = _context.Database.GetDbConnection();

            var result =
                await connection.QueryFirstOrDefaultAsync<string>(
                    "sp_GenerateAdmissionNumber",
                    commandType: CommandType.StoredProcedure);

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new Exception(
                    "Failed to generate admission number.");
            }

            return result;
        }
    }
}