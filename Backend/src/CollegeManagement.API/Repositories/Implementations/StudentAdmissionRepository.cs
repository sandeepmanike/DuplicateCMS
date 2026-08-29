using System.Data;
using Dapper;
using CollegeManagement.API.DTOs.StudentAdmission;
using CollegeManagement.API.Repositories.Interfaces;

namespace CollegeManagement.API.Repositories.Implementations
{
    public class StudentAdmissionRepository
        : IStudentAdmissionRepository
    {
        private readonly IDbConnection _connection;

        public StudentAdmissionRepository(IDbConnection connection)
        {
            _connection = connection;
        }


        // =====================================================
        // CREATE ADMISSION
        // =====================================================

        public async Task<StudentAdmissionResponseDto> CreateAsync(
            CreateStudentAdmissionRequest request,
            string? studentPhoto)
        {
            var result =
                await _connection.QueryFirstOrDefaultAsync<StudentAdmissionResponseDto>(
                    "sp_CreateStudentAdmission",
                    new
                    {
                        p_AdmissionDate = request.AdmissionDate,
                        p_AdmissionType = request.AdmissionType,
                        p_AdmissionQuota = request.AdmissionQuota,

                        // Academic
                        p_BoardId = request.BoardId,
                        p_AcademicYearId = request.AcademicYearId,
                        p_AcademicLevelId = request.AcademicLevelId,
                        p_GroupId = request.GroupId,
                        p_ProgramId = request.ProgramId,

                        // Student
                        p_FirstName = request.FirstName,
                        p_LastName = request.LastName,
                        p_Gender = request.Gender,
                        p_DateOfBirth = request.DateOfBirth,
                        p_BloodGroup = request.BloodGroup,
                        p_StudentEmail = request.StudentEmail,
                        p_StudentMobileNumber =
                            request.StudentMobileNumber,

                        // Photo
                        p_StudentPhoto = studentPhoto,

                        // Personal
                        p_AadhaarNumber = request.AadhaarNumber,
                        p_Nationality = request.Nationality,
                        p_Religion = request.Religion,
                        p_Category = request.Category,

                        // Father
                        p_FatherName = request.FatherName,
                        p_FatherOccupation =
                            request.FatherOccupation,
                        p_FatherMobile = request.FatherMobile,
                        p_FatherEmail = request.FatherEmail,

                        // Mother
                        p_MotherName = request.MotherName,
                        p_MotherOccupation =
                            request.MotherOccupation,
                        p_MotherMobile = request.MotherMobile,
                        p_MotherEmail = request.MotherEmail,

                        // Guardian
                        p_GuardianName = request.GuardianName,
                        p_GuardianMobile =
                            request.GuardianMobile,
                        p_GuardianEmail =
                            request.GuardianEmail,

                        // Other
                        p_AnnualIncome = request.AnnualIncome,
                        p_ScholarshipStatus =
                            request.ScholarshipStatus,

                        // Address
                        p_Address = request.Address,
                        p_City = request.City,
                        p_District = request.District,
                        p_State = request.State,
                        p_Pincode = request.Pincode,

                        // Previous Education
                        p_PreviousSchool =
                            request.PreviousSchool,
                        p_PreviousBoard =
                            request.PreviousBoard,
                        p_PreviousPercentage =
                            request.PreviousPercentage,
                        p_PreviousYearOfPassing =
                            request.PreviousYearOfPassing,

                        // Academic
                        p_Medium = request.Medium,
                        p_SecondLanguage =
                            request.SecondLanguage
                    },
                    commandType: CommandType.StoredProcedure);

            if (result == null)
            {
                throw new Exception(
                    "Student admission could not be created.");
            }

            return result;
        }


        // =====================================================
        // GET BY ID
        // =====================================================

        public async Task<StudentAdmissionResponseDto?>
            GetByIdAsync(int admissionId)
        {
            return await _connection
                .QueryFirstOrDefaultAsync<StudentAdmissionResponseDto>(
                    "sp_GetStudentAdmissionById",
                    new
                    {
                        p_AdmissionId = admissionId
                    },
                    commandType: CommandType.StoredProcedure);
        }


        // =====================================================
        // GET ALL
        // =====================================================

        public async Task<IEnumerable<StudentAdmissionResponseDto>>
            GetAllAsync()
        {
            return await _connection
                .QueryAsync<StudentAdmissionResponseDto>(
                    "sp_GetAllStudentAdmissions",
                    commandType: CommandType.StoredProcedure);
        }


        // =====================================================
        // UPDATE
        // =====================================================

        public async Task<StudentAdmissionResponseDto?>
            UpdateAsync(
                int admissionId,
                UpdateStudentAdmissionRequest request,
                string? studentPhoto)
        {
            return await _connection
                .QueryFirstOrDefaultAsync<StudentAdmissionResponseDto>(
                    "sp_UpdateStudentAdmission",
                    new
                    {
                        p_AdmissionId = admissionId,

                        p_AdmissionDate =
                            request.AdmissionDate,

                        p_AdmissionType =
                            request.AdmissionType,

                        p_AdmissionQuota =
                            request.AdmissionQuota,

                        // Academic
                        p_BoardId = request.BoardId,
                        p_AcademicYearId =
                            request.AcademicYearId,
                        p_AcademicLevelId =
                            request.AcademicLevelId,
                        p_GroupId = request.GroupId,
                        p_ProgramId = request.ProgramId,

                        // Student
                        p_FirstName = request.FirstName,
                        p_LastName = request.LastName,
                        p_Gender = request.Gender,
                        p_DateOfBirth =
                            request.DateOfBirth,
                        p_BloodGroup =
                            request.BloodGroup,
                        p_StudentEmail =
                            request.StudentEmail,
                        p_StudentMobileNumber =
                            request.StudentMobileNumber,

                        // Photo
                        p_StudentPhoto = studentPhoto,

                        // Personal
                        p_AadhaarNumber =
                            request.AadhaarNumber,
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

                        // Other
                        p_AnnualIncome =
                            request.AnnualIncome,
                        p_ScholarshipStatus =
                            request.ScholarshipStatus,

                        // Address
                        p_Address = request.Address,
                        p_City = request.City,
                        p_District = request.District,
                        p_State = request.State,
                        p_Pincode = request.Pincode,

                        // Previous Education
                        p_PreviousSchool =
                            request.PreviousSchool,
                        p_PreviousBoard =
                            request.PreviousBoard,
                        p_PreviousPercentage =
                            request.PreviousPercentage,
                        p_PreviousYearOfPassing =
                            request.PreviousYearOfPassing,

                        p_Medium = request.Medium,
                        p_SecondLanguage =
                            request.SecondLanguage
                    },
                    commandType: CommandType.StoredProcedure);
        }


        // =====================================================
        // VERIFY
        // =====================================================

        public async Task<bool> VerifyAsync(
            VerifyStudentAdmissionRequest request)
        {
            var result =
                await _connection.QuerySingleOrDefaultAsync<int>(
                    "sp_VerifyStudentAdmission",
                    new
                    {
                        p_AdmissionId =
                            request.AdmissionId
                    },
                    commandType: CommandType.StoredProcedure);

            return result > 0;
        }


        // =====================================================
        // APPROVE
        // =====================================================

        public async Task<bool> ApproveAsync(
            ApproveStudentAdmissionRequest request)
        {
            var result =
                await _connection.QuerySingleOrDefaultAsync<int>(
                    "sp_ApproveStudentAdmission",
                    new
                    {
                        p_AdmissionId =
                            request.AdmissionId
                    },
                    commandType: CommandType.StoredProcedure);

            return result > 0;
        }


        // =====================================================
        // REJECT
        // =====================================================

        public async Task<bool> RejectAsync(
            RejectStudentAdmissionRequest request)
        {
            var result =
                await _connection.QuerySingleOrDefaultAsync<int>(
                    "sp_RejectStudentAdmission",
                    new
                    {
                        p_AdmissionId =
                            request.AdmissionId,

                        p_RejectionReason =
                            request.RejectionReason,

                        p_Remarks =
                            request.Remarks
                    },
                    commandType: CommandType.StoredProcedure);

            return result > 0;
        }


        // =====================================================
        // SINGLE SECTION ALLOCATION
        // =====================================================

        public async Task<bool> AllocateSectionAsync(
            AllocateSectionRequest request)
        {
            var result =
                await _connection.QuerySingleOrDefaultAsync<int>(
                    "sp_AllocateStudentSection",
                    new
                    {
                        p_AdmissionId =
                            request.AdmissionId,

                        p_SectionId =
                            request.SectionId
                    },
                    commandType: CommandType.StoredProcedure);

            return result > 0;
        }


        // =====================================================
        // BULK SECTION ALLOCATION
        // =====================================================

        public async Task<int> BulkAllocateSectionAsync(
            BulkSectionAllocationRequest request)
        {
            var total = 0;

            foreach (var admissionId in request.AdmissionIds)
            {
                var result =
                    await _connection
                        .QuerySingleOrDefaultAsync<int>(
                            "sp_AllocateStudentSection",
                            new
                            {
                                p_AdmissionId = admissionId,
                                p_SectionId =
                                    request.SectionId
                            },
                            commandType:
                                CommandType.StoredProcedure);

                total += result;
            }

            return total;
        }
        //generatenumber//
        // =====================================================
        // GENERATE ADMISSION NUMBER
        // =====================================================

        // =====================================================
        // GENERATE ADMISSION NUMBER
        // =====================================================

        // =====================================================
        // GENERATE ADMISSION NUMBER
        // =====================================================

        public async Task<string> GenerateAdmissionNumberAsync()
        {
            const string sql = @"
        SELECT CONCAT(
            'ADM-',
            LPAD(
                COALESCE(
                    MAX(
                        CAST(
                            SUBSTRING(AdmissionNo, 5) AS UNSIGNED
                        )
                    ),
                    0
                ) + 1,
                2,
                '0'
            )
        )
        FROM StudentAdmissions
        WHERE AdmissionNo LIKE 'ADM-%';";

            return await _connection.QuerySingleAsync<string>(sql);
        }

        // =====================================================
        // BULK ROLL NUMBER ALLOCATION
        // =====================================================

        public async Task<int> BulkAllocateRollNumbersAsync(
            BulkRollNumberAllocationRequest request)
        {
            var total = 0;

            var rollNumber =
                request.StartingRollNumber;

            foreach (var admissionId
                     in request.AdmissionIds)
            {
                var result =
                    await _connection
                        .QuerySingleOrDefaultAsync<int>(
                            "sp_AllocateStudentRollNumber",
                            new
                            {
                                p_AdmissionId =
                                    admissionId,

                                p_SectionId =
                                    request.SectionId,

                                p_RollNo =
                                    rollNumber.ToString()
                            },
                            commandType:
                                CommandType.StoredProcedure);

                if (result > 0)
                    total++;

                rollNumber++;
            }

            return total;
        }
    }
}