using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.StudentAdmission;
using CollegeManagement.API.Repositories.Interfaces;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace CollegeManagement.API.Repositories.Implementations
{
    public partial class StudentAdmissionRepository : IStudentAdmissionRepository
    {
        private readonly AppDbContext _context;

        public StudentAdmissionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentAdmissionResponseDto>> GetAllAsync()
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            return await connection.QueryAsync<StudentAdmissionResponseDto>(
                "sp_GetAllAdmissions",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<StudentAdmissionResponseDto?> GetByIdAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            return await connection.QueryFirstOrDefaultAsync<StudentAdmissionResponseDto>(
                "sp_GetAdmissionById",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);
        }
        private async Task<string?> SaveFileAsync(IFormFile? file, string folderName)
        {
            if (file == null || file.Length == 0)
                return null;

            var uploadsRoot = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "uploads",
                folderName);

            if (!Directory.Exists(uploadsRoot))
                Directory.CreateDirectory(uploadsRoot);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(uploadsRoot, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", folderName, fileName)
                .Replace("\\", "/");
        }

        private async Task<AdmissionFiles> SaveAdmissionFilesAsync(
            CreateStudentAdmissionRequest request)
        {
            return new AdmissionFiles
            {
                StudentPhoto = await SaveFileAsync(
                    request.StudentPhoto,
                    "StudentPhotos"),

                BirthCertificate = await SaveFileAsync(
                    request.BirthCertificate,
                    "BirthCertificates"),

                TransferCertificate = await SaveFileAsync(
                    request.TransferCertificate,
                    "TransferCertificates"),

                StudyCertificate = await SaveFileAsync(
                    request.StudyCertificate,
                    "StudyCertificates"),

                AadhaarDocument = await SaveFileAsync(
                    request.AadhaarDocument,
                    "AadhaarDocuments"),

                CommunityCertificate = await SaveFileAsync(
                    request.CommunityCertificate,
                    "CommunityCertificates"),

                IncomeCertificate = await SaveFileAsync(
                    request.IncomeCertificate,
                    "IncomeCertificates"),

                PassportPhoto = await SaveFileAsync(
                    request.PassportPhoto,
                    "PassportPhotos")
            };
        }

        private class AdmissionFiles
        {
            public string? StudentPhoto { get; set; }
            public string? BirthCertificate { get; set; }
            public string? TransferCertificate { get; set; }
            public string? StudyCertificate { get; set; }
            public string? AadhaarDocument { get; set; }
            public string? CommunityCertificate { get; set; }
            public string? IncomeCertificate { get; set; }
            public string? PassportPhoto { get; set; }
        }
        public async Task<StudentAdmissionResponseDto> CreateAsync(
    CreateStudentAdmissionRequest request)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var files = await SaveAdmissionFilesAsync(request);

            return await connection.QueryFirstAsync<StudentAdmissionResponseDto>(
                "sp_CreateAdmission",
                new
                {
                    p_AdmissionNo = request.AdmissionNo,
                    p_AdmissionDate = request.AdmissionDate,

                    p_FirstName = request.FirstName,
                    p_LastName = request.LastName,
                    p_Gender = request.Gender,
                    p_DateOfBirth = request.DateOfBirth,
                    p_BloodGroup = request.BloodGroup,

                    p_StudentPhoto = files.StudentPhoto,

                    p_AadhaarNumber = request.AadhaarNumber,
                    p_Nationality = request.Nationality,
                    p_Religion = request.Religion,
                    p_Category = request.Category,

                    p_FatherName = request.FatherName,
                    p_MotherName = request.MotherName,
                    p_GuardianName = request.GuardianName,

                    p_ParentMobile = request.ParentMobile,
                    p_ParentEmail = request.ParentEmail,
                    p_Occupation = request.Occupation,
                    p_AnnualIncome = request.AnnualIncome,

                    p_Address = request.Address,
                    p_City = request.City,
                    p_District = request.District,
                    p_State = request.State,
                    p_Pincode = request.Pincode,

                    p_BoardId = request.BoardId,
                    p_AcademicYearId = request.AcademicYearId,
                    p_AcademicLevel = request.AcademicLevel,
                    p_GroupId = request.GroupId,
                    p_SectionId = request.SectionId,

                    p_PreviousSchool = request.PreviousSchool,
                    p_PreviousBoard = request.PreviousBoard,
                    p_PreviousPercentage = request.PreviousPercentage,

                    p_BirthCertificate = files.BirthCertificate,
                    p_TransferCertificate = files.TransferCertificate,
                    p_StudyCertificate = files.StudyCertificate,
                    p_AadhaarDocument = files.AadhaarDocument,
                    p_CommunityCertificate = files.CommunityCertificate,
                    p_IncomeCertificate = files.IncomeCertificate,
                    p_PassportPhoto = files.PassportPhoto
                },
        commandType: CommandType.StoredProcedure);
        }
        public async Task<StudentAdmissionResponseDto?> UpdateAsync(
    int admissionId,
    UpdateStudentAdmissionRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await connection.ExecuteAsync(
                "sp_DeleteAdmission",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> VerifyAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await connection.ExecuteAsync(
                "sp_VerifyAdmission",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> ApproveAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await connection.ExecuteAsync(
                "sp_ApproveAdmission",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<bool> RejectAsync(int admissionId)
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            var result = await connection.ExecuteAsync(
                "sp_RejectAdmission",
                new
                {
                    p_AdmissionId = admissionId
                },
                commandType: CommandType.StoredProcedure);

            return result > 0;
        }

        public async Task<string> GenerateAdmissionNumberAsync()
        {
            var connection = _context.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
                await connection.OpenAsync();

            return await connection.QueryFirstAsync<string>(
                "sp_GenerateAdmissionNumber",
                commandType: CommandType.StoredProcedure);
        }
    }
}