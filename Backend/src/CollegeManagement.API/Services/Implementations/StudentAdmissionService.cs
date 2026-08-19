using CollegeManagement.API.DTOs.StudentAdmission;
using CollegeManagement.API.Repositories.Implementations;
using CollegeManagement.API.Repositories.Interfaces;
using CollegeManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace CollegeManagement.API.Services.Implementations
{
    public class StudentAdmissionService : IStudentAdmissionService
    {
        private readonly IStudentAdmissionRepository _repository;
        private readonly IWebHostEnvironment _environment;

        private static readonly string[] AllowedExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".pdf"
        };

        private const long MaxFileSize = 2 * 1024 * 1024; // 2 MB

        public StudentAdmissionService(
            IStudentAdmissionRepository repository,
            IWebHostEnvironment environment)
        {
            _repository = repository;
            _environment = environment;
        }


        // =========================================================
        // GET ALL
        // =========================================================

        public async Task<IEnumerable<StudentAdmissionResponseDto>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }


        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<StudentAdmissionResponseDto?> GetByIdAsync(
            int admissionId)
        {
            if (admissionId <= 0)
            {
                throw new ArgumentException(
                    "Invalid admission ID.");
            }

            return await _repository.GetByIdAsync(admissionId);
        }


        // =========================================================
        // CREATE
        // =========================================================

        public async Task<StudentAdmissionResponseDto> CreateAsync(
            CreateStudentAdmissionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            ValidateCreateRequest(request);

            // -----------------------------------------------------
            // SAVE STUDENT PHOTO
            // -----------------------------------------------------

            var studentPhoto = await SaveFileAsync(
                request.StudentPhoto,
                "student-photos",
                allowPdf: false);


            // -----------------------------------------------------
            // SAVE DOCUMENTS
            // -----------------------------------------------------

            var birthCertificate = await SaveFileAsync(
                request.BirthCertificate,
                "documents");

            var transferCertificate = await SaveFileAsync(
                request.TransferCertificate,
                "documents");

            var studyCertificate = await SaveFileAsync(
                request.StudyCertificate,
                "documents");

            var aadhaarDocument = await SaveFileAsync(
                request.AadhaarDocument,
                "documents");

            var communityCertificate = await SaveFileAsync(
                request.CommunityCertificate,
                "documents");

            var incomeCertificate = await SaveFileAsync(
                request.IncomeCertificate,
                "documents");

            var casteCertificate = await SaveFileAsync(
                request.CasteCertificate,
                "documents");

            var tenthCertificate = await SaveFileAsync(
                request.TenthCertificate,
                "documents");

            var marksMemo = await SaveFileAsync(
                request.MarksMemo,
                "documents");


            // -----------------------------------------------------
            // DATABASE
            // -----------------------------------------------------

            return await _repository.CreateAsync(
                request,
                studentPhoto,
                birthCertificate,
                transferCertificate,
                studyCertificate,
                aadhaarDocument,
                communityCertificate,
                incomeCertificate,
                casteCertificate,
                tenthCertificate,
                marksMemo);
        }


        // =========================================================
        // UPDATE
        // =========================================================

        public async Task<StudentAdmissionResponseDto?> UpdateAsync(
            int admissionId,
            UpdateStudentAdmissionRequest request)
        {
            if (admissionId <= 0)
            {
                throw new ArgumentException(
                    "Invalid admission ID.");
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            // -----------------------------------------------------
            // CHECK EXISTING ADMISSION
            // -----------------------------------------------------

            var existing =
                await _repository.GetByIdAsync(admissionId);

            if (existing == null)
            {
                return null;
            }

            // -----------------------------------------------------
            // VALIDATE
            // -----------------------------------------------------

            ValidateUpdateRequest(request);


            // -----------------------------------------------------
            // SAVE NEW FILES ONLY
            // -----------------------------------------------------

            string? studentPhoto = null;

            if (request.StudentPhoto != null)
            {
                studentPhoto = await SaveFileAsync(
                    request.StudentPhoto,
                    "student-photos",
                    allowPdf: false);
            }


            string? birthCertificate = null;

            if (request.BirthCertificate != null)
            {
                birthCertificate = await SaveFileAsync(
                    request.BirthCertificate,
                    "documents");
            }


            string? transferCertificate = null;

            if (request.TransferCertificate != null)
            {
                transferCertificate = await SaveFileAsync(
                    request.TransferCertificate,
                    "documents");
            }


            string? studyCertificate = null;

            if (request.StudyCertificate != null)
            {
                studyCertificate = await SaveFileAsync(
                    request.StudyCertificate,
                    "documents");
            }


            string? aadhaarDocument = null;

            if (request.AadhaarDocument != null)
            {
                aadhaarDocument = await SaveFileAsync(
                    request.AadhaarDocument,
                    "documents");
            }


            string? communityCertificate = null;

            if (request.CommunityCertificate != null)
            {
                communityCertificate = await SaveFileAsync(
                    request.CommunityCertificate,
                    "documents");
            }


            string? incomeCertificate = null;

            if (request.IncomeCertificate != null)
            {
                incomeCertificate = await SaveFileAsync(
                    request.IncomeCertificate,
                    "documents");
            }


            string? casteCertificate = null;

            if (request.CasteCertificate != null)
            {
                casteCertificate = await SaveFileAsync(
                    request.CasteCertificate,
                    "documents");
            }


            string? tenthCertificate = null;

            if (request.TenthCertificate != null)
            {
                tenthCertificate = await SaveFileAsync(
                    request.TenthCertificate,
                    "documents");
            }


            string? marksMemo = null;

            if (request.MarksMemo != null)
            {
                marksMemo = await SaveFileAsync(
                    request.MarksMemo,
                    "documents");
            }


            // -----------------------------------------------------
            // DATABASE
            // -----------------------------------------------------

            return await _repository.UpdateAsync(
                admissionId,
                request,
                studentPhoto,
                birthCertificate,
                transferCertificate,
                studyCertificate,
                aadhaarDocument,
                communityCertificate,
                incomeCertificate,
                casteCertificate,
                tenthCertificate,
                marksMemo);
        }


        // =========================================================
        // DELETE / SOFT DELETE
        // =========================================================

        public async Task<bool> DeleteAsync(
            int admissionId)
        {
            if (admissionId <= 0)
            {
                throw new ArgumentException(
                    "Invalid admission ID.");
            }

            return await _repository.DeleteAsync(admissionId);
        }


        //validation//
        public async Task<dynamic?> SubmitAsync(int admissionId)
        {
            if (admissionId <= 0)
                throw new ArgumentException("Invalid AdmissionId.");

            return await _repository
                .SubmitAsync(admissionId);
        }


        // =========================================================
        // VERIFY
        // =========================================================

        public async Task<bool> VerifyAsync(
            int admissionId)
        {
            if (admissionId <= 0)
            {
                throw new ArgumentException(
                    "Invalid admission ID.");
            }

            return await _repository.VerifyAsync(admissionId);
        }


        // =========================================================
        // APPROVE
        // Creates Student + generates RollNo
        // =========================================================

        public async Task<AdmissionApprovalResponseDto?> ApproveAsync(
            int admissionId)
        {
            if (admissionId <= 0)
            {
                throw new ArgumentException(
                    "Invalid admission ID.");
            }

            return await _repository.ApproveAsync(
                admissionId);
        }


        // =========================================================
        // REJECT
        // =========================================================

        public async Task<bool> RejectAsync(
            int admissionId)
        {
            if (admissionId <= 0)
            {
                throw new ArgumentException(
                    "Invalid admission ID.");
            }

            return await _repository.RejectAsync(
                admissionId);
        }
        //GENERATE//
        // =========================================================
        // GENERATE ADMISSION NUMBER
        // =========================================================

        public async Task<string> GenerateAdmissionNumberAsync()
        {
            return await _repository.GenerateAdmissionNumberAsync();
        }


        // =========================================================
        // CREATE VALIDATION
        // =========================================================

        private static void ValidateCreateRequest(
            CreateStudentAdmissionRequest request)
        {
            if (request.AdmissionDate == default)
            {
                throw new ArgumentException(
                    "Admission date is required.");
            }

            if (request.AdmissionDate.Date >
                DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Admission date cannot be in the future.");
            }


            if (string.IsNullOrWhiteSpace(
                request.FirstName))
            {
                throw new ArgumentException(
                    "First name is required.");
            }


            if (string.IsNullOrWhiteSpace(
                request.Gender))
            {
                throw new ArgumentException(
                    "Gender is required.");
            }


            if (request.DateOfBirth == default)
            {
                throw new ArgumentException(
                    "Date of birth is required.");
            }


            if (request.DateOfBirth.Date >
                DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Date of birth cannot be a future date.");
            }


            if (string.IsNullOrWhiteSpace(
                request.FatherName))
            {
                throw new ArgumentException(
                    "Father name is required.");
            }


            if (string.IsNullOrWhiteSpace(
                request.MotherName))
            {
                throw new ArgumentException(
                    "Mother name is required.");
            }


            if (request.AnnualIncome.HasValue &&
                request.AnnualIncome.Value < 0)
            {
                throw new ArgumentException(
                    "Annual income cannot be negative.");
            }


            if (request.PreviousPercentage.HasValue &&
                (request.PreviousPercentage.Value < 0 ||
                 request.PreviousPercentage.Value > 100))
            {
                throw new ArgumentException(
                    "Previous percentage must be between 0 and 100.");
            }


            if (request.PreviousYearOfPassing.HasValue &&
                (request.PreviousYearOfPassing.Value < 2000 ||
                 request.PreviousYearOfPassing.Value >
                 DateTime.UtcNow.Year))
            {
                throw new ArgumentException(
                    "Invalid previous year of passing.");
            }


            ValidateAcademicIds(
                request.BoardId,
                request.AcademicYearId,
                request.AcademicLevelId,
                request.GroupId,
                request.SectionId);
        }


        // =========================================================
        // UPDATE VALIDATION
        // =========================================================

        private static void ValidateUpdateRequest(
            UpdateStudentAdmissionRequest request)
        {
            if (request.AdmissionDate == default)
            {
                throw new ArgumentException(
                    "Admission date is required.");
            }


            if (request.AdmissionDate.Date >
                DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Admission date cannot be in the future.");
            }


            if (string.IsNullOrWhiteSpace(
                request.FirstName))
            {
                throw new ArgumentException(
                    "First name is required.");
            }


            if (string.IsNullOrWhiteSpace(
                request.Gender))
            {
                throw new ArgumentException(
                    "Gender is required.");
            }


            if (request.DateOfBirth == default)
            {
                throw new ArgumentException(
                    "Date of birth is required.");
            }


            if (request.DateOfBirth.Date >
                DateTime.UtcNow.Date)
            {
                throw new ArgumentException(
                    "Date of birth cannot be a future date.");
            }


            if (string.IsNullOrWhiteSpace(
                request.FatherName))
            {
                throw new ArgumentException(
                    "Father name is required.");
            }


            if (string.IsNullOrWhiteSpace(
                request.MotherName))
            {
                throw new ArgumentException(
                    "Mother name is required.");
            }


            if (request.AnnualIncome.HasValue &&
                request.AnnualIncome.Value < 0)
            {
                throw new ArgumentException(
                    "Annual income cannot be negative.");
            }


            if (request.PreviousPercentage.HasValue &&
                (request.PreviousPercentage.Value < 0 ||
                 request.PreviousPercentage.Value > 100))
            {
                throw new ArgumentException(
                    "Previous percentage must be between 0 and 100.");
            }


            if (request.PreviousYearOfPassing.HasValue &&
                (request.PreviousYearOfPassing.Value < 2000 ||
                 request.PreviousYearOfPassing.Value >
                 DateTime.UtcNow.Year))
            {
                throw new ArgumentException(
                    "Invalid previous year of passing.");
            }


            ValidateAcademicIds(
                request.BoardId,
                request.AcademicYearId,
                request.AcademicLevelId,
                request.GroupId,
                request.SectionId);
        }


        // =========================================================
        // ACADEMIC VALIDATION
        // =========================================================

        private static void ValidateAcademicIds(
            int boardId,
            int academicYearId,
            int academicLevelId,
            int groupId,
            int sectionId)
        {
            if (boardId <= 0)
            {
                throw new ArgumentException(
                    "Board is required.");
            }


            if (academicYearId <= 0)
            {
                throw new ArgumentException(
                    "Academic Year is required.");
            }


            if (academicLevelId <= 0)
            {
                throw new ArgumentException(
                    "Academic Level is required.");
            }


            if (groupId <= 0)
            {
                throw new ArgumentException(
                    "Group is required.");
            }


            if (sectionId <= 0)
            {
                throw new ArgumentException(
                    "Section is required.");
            }
        }


        // =========================================================
        // FILE UPLOAD
        // =========================================================

        private async Task<string?> SaveFileAsync(
            IFormFile? file,
            string folder,
            bool allowPdf = true)
        {
            if (file == null ||
                file.Length == 0)
            {
                return null;
            }


            // -----------------------------------------------------
            // FILE SIZE
            // -----------------------------------------------------

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException(
                    $"{file.FileName} exceeds the maximum file size of 2 MB.");
            }


            // -----------------------------------------------------
            // FILE EXTENSION
            // -----------------------------------------------------

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();


            if (!AllowedExtensions.Contains(extension))
            {
                throw new ArgumentException(
                    $"File type {extension} is not allowed.");
            }


            // -----------------------------------------------------
            // PHOTO SHOULD NOT BE PDF
            // -----------------------------------------------------

            if (!allowPdf &&
                extension == ".pdf")
            {
                throw new ArgumentException(
                    "Student photo must be JPG, JPEG, or PNG.");
            }


            // -----------------------------------------------------
            // ROOT PATH
            // -----------------------------------------------------

            var rootPath =
                _environment.WebRootPath;

            if (string.IsNullOrWhiteSpace(rootPath))
            {
                rootPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot");
            }


            // -----------------------------------------------------
            // UPLOAD FOLDER
            // -----------------------------------------------------

            var uploadFolder =
                Path.Combine(
                    rootPath,
                    "Uploads",
                    folder);

            Directory.CreateDirectory(
                uploadFolder);


            // -----------------------------------------------------
            // UNIQUE FILE NAME
            // -----------------------------------------------------

            var fileName =
                $"{Guid.NewGuid():N}{extension}";


            var physicalPath =
                Path.Combine(
                    uploadFolder,
                    fileName);


            // -----------------------------------------------------
            // SAVE FILE
            // -----------------------------------------------------

            await using var stream =
                new FileStream(
                    physicalPath,
                    FileMode.Create);

            await file.CopyToAsync(stream);


            // -----------------------------------------------------
            // RETURN RELATIVE PATH
            // -----------------------------------------------------

            return $"/Uploads/{folder}/{fileName}";
        }
    }
}