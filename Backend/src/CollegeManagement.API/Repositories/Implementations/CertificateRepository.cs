using Dapper;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Certificate;
using CollegeManagement.API.Repositories.Interfaces;
using System.Data;

namespace CollegeManagement.API.Repositories.Implementations;

public class CertificateRepository : ICertificateRepository
{
    private readonly DatabaseContext _database;

    public CertificateRepository(DatabaseContext database)
    {
        _database = database;
    }

    // =========================================================
    // GET ALL
    // =========================================================

    public async Task<IReadOnlyList<CertificateResponseDto>> GetAllAsync(
        string? search,
        string? status,
        CancellationToken ct = default)
    {
        using var connection = _database.CreateConnection();

        var rows = await connection.QueryAsync<CertificateDbRow>(
            new CommandDefinition(
                "sp_GetCertificates",
                commandType: CommandType.StoredProcedure,
                cancellationToken: ct));

        var result = rows
            .Select(MapToDto)
            .ToList();

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();

            result = result
                .Where(x =>
                    Contains(x.CertificateNumber, search) ||
                    Contains(x.AdmissionNo, search) ||
                    Contains(x.StudentName, search) ||
                    Contains(x.CertificateType, search))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            status = status.Trim();

            result = result
                .Where(x =>
                    string.Equals(
                        x.Status,
                        status,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return result;
    }

    // =========================================================
    // GET BY ID
    // =========================================================

    public async Task<CertificateResponseDto?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        using var connection = _database.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "p_CertificateId",
            id,
            DbType.Int32);

        var row =
            await connection.QueryFirstOrDefaultAsync<CertificateDbRow>(
                new CommandDefinition(
                    "sp_GetCertificateById",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        return row == null
            ? null
            : MapToDto(row);
    }

    // =========================================================
    // GENERATE CERTIFICATE
    // =========================================================

    public async Task<CertificateResponseDto?> GenerateAsync(
        GenerateCertificateDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
            return null;

        if (string.IsNullOrWhiteSpace(request.CertificateType))
            return null;

        var certificateType =
            request.CertificateType.Trim();

        var certificateTypeLower =
            certificateType.ToLowerInvariant();

        string procedureName;
        bool isOtherCertificate = false;

        // =========================================================
        // SELECT STORED PROCEDURE
        // =========================================================

        switch (certificateTypeLower)
        {
            // -----------------------------------------------------
            // BONAFIDE
            // -----------------------------------------------------

            case "bonafide":
            case "bonafide certificate":

                procedureName =
                    "sp_GenerateBonafideCertificate";

                break;


            // -----------------------------------------------------
            // STUDY
            // -----------------------------------------------------

            case "study":
            case "study certificate":

                procedureName =
                    "sp_GenerateStudyCertificate";

                break;


            // -----------------------------------------------------
            // CONDUCT
            // -----------------------------------------------------

            case "conduct":
            case "conduct certificate":

                procedureName =
                    "sp_GenerateConductCertificate";

                break;


            // -----------------------------------------------------
            // TRANSFER CERTIFICATE
            // -----------------------------------------------------

            case "tc":
            case "transfer":
            case "transfer certificate":

                procedureName =
                    "sp_GenerateTCCertificate";

                break;


            // -----------------------------------------------------
            // EVERYTHING ELSE
            // -----------------------------------------------------

            default:

                procedureName =
                    "sp_GenerateOtherCertificate";

                isOtherCertificate = true;

                break;
        }


        // =========================================================
        // ISSUE DATE
        // =========================================================

        var issueDate =
            request.IssueDate == default
                ? DateTime.Now
                : request.IssueDate;


        // =========================================================
        // PARAMETERS
        // =========================================================

        var parameters =
            new DynamicParameters();

        parameters.Add(
            "p_AdmissionNo",
            request.AdmissionNo.Trim(),
            DbType.String);

        parameters.Add(
            "p_Purpose",
            request.Purpose?.Trim(),
            DbType.String);

        parameters.Add(
            "p_IssueDate",
            issueDate,
            DbType.DateTime);

        parameters.Add(
            "p_Remarks",
            request.Remarks?.Trim(),
            DbType.String);


        // =========================================================
        // OTHER CERTIFICATE
        // SEND ACTUAL CERTIFICATE TYPE
        // =========================================================

        if (isOtherCertificate)
        {
            parameters.Add(
                "p_CertificateType",
                certificateType,
                DbType.String);
        }


        // =========================================================
        // EXECUTE STORED PROCEDURE
        // =========================================================

        using var connection =
            _database.CreateConnection();

        var row =
            await connection.QueryFirstOrDefaultAsync<CertificateDbRow>(
                new CommandDefinition(
                    procedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        if (row == null)
            return null;

        return MapToDto(row);
    }
    // =========================================================
    // UPDATE BY ADMISSION NO
    // =========================================================

    public async Task<CertificateResponseDto?> UpdateByAdmissionNoAsync(
        UpdateCertificateDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
            return null;

        using var connection = _database.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "p_AdmissionNo",
            request.AdmissionNo.Trim(),
            DbType.String);

        parameters.Add(
            "p_CertificateType",
            request.CertificateType?.Trim(),
            DbType.String);

        parameters.Add(
            "p_Purpose",
            request.Purpose?.Trim(),
            DbType.String);

        parameters.Add(
            "p_IssueDate",
            request.IssueDate,
            DbType.DateTime);

        parameters.Add(
            "p_Remarks",
            request.Remarks?.Trim(),
            DbType.String);

        var row =
            await connection.QueryFirstOrDefaultAsync<CertificateDbRow>(
                new CommandDefinition(
                    "sp_UpdateCertificateByAdmissionNo",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        return row == null
            ? null
            : MapToDto(row);
    }

    // =========================================================
    // HISTORY
    // =========================================================

    public async Task<IReadOnlyList<CertificateResponseDto>> GetHistoryAsync(
        string? admissionNo,
        CancellationToken ct = default)
    {
        using var connection = _database.CreateConnection();

        var rows =
            await connection.QueryAsync<CertificateDbRow>(
                new CommandDefinition(
                    "sp_GetCertificateHistory",
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        var result = rows
            .Select(MapToDto)
            .ToList();

        if (!string.IsNullOrWhiteSpace(admissionNo))
        {
            admissionNo = admissionNo.Trim();

            result = result
                .Where(x =>
                    string.Equals(
                        x.AdmissionNo,
                        admissionNo,
                        StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return result;
    }

    // =========================================================
    // VERIFY
    // =========================================================

    public async Task<CertificateResponseDto?> VerifyAsync(
        string certificateNo,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(certificateNo))
            return null;

        using var connection = _database.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "p_CertificateNumber",
            certificateNo.Trim(),
            DbType.String);

        var row =
            await connection.QueryFirstOrDefaultAsync<CertificateDbRow>(
                new CommandDefinition(
                    "sp_VerifyCertificate",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        return row == null
            ? null
            : MapToDto(row);
    }

    // =========================================================
    // REISSUE
    // =========================================================

    public async Task<CertificateResponseDto?> ReissueAsync(
        ReissueCertificateDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.AdmissionNo))
            return null;

        if (string.IsNullOrWhiteSpace(request.CertificateType))
            return null;

        using var connection = _database.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "p_AdmissionNo",
            request.AdmissionNo.Trim(),
            DbType.String);

        parameters.Add(
            "p_CertificateType",
            request.CertificateType.Trim(),
            DbType.String);

        parameters.Add(
            "p_Purpose",
            request.Purpose?.Trim(),
            DbType.String);

        parameters.Add(
            "p_RequestDate",
            request.RequestDate == default
                ? DateTime.Now
                : request.RequestDate,
            DbType.DateTime);

        parameters.Add(
            "p_Remarks",
            request.Remarks?.Trim(),
            DbType.String);

        var row =
            await connection.QueryFirstOrDefaultAsync<CertificateDbRow>(
                new CommandDefinition(
                    "sp_ReissueCertificate",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        if (row == null)
            return null;

        return MapToDto(row);
    }

    // =========================================================
    // CANCEL
    // =========================================================

    public async Task<bool> CancelAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
            return false;

        using var connection = _database.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "p_CertificateId",
            id,
            DbType.Int32);

        var result =
            await connection.QueryFirstOrDefaultAsync<CertificateDbRow>(
                new CommandDefinition(
                    "sp_CancelCertificate",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        return result != null;
    }

    // =========================================================
    // MOVE STATUS
    // =========================================================

    public async Task<bool> MoveStatusAsync(
        int id,
        string status,
        string? issuedBy = null,
        CancellationToken ct = default)
    {
        if (id <= 0)
            return false;

        if (string.IsNullOrWhiteSpace(status))
            return false;

        using var connection = _database.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "p_CertificateId",
            id,
            DbType.Int32);

        parameters.Add(
            "p_NewStatus",
            status.Trim(),
            DbType.String);

        parameters.Add(
            "p_IssuedBy",
            issuedBy,
            DbType.String);

        var result =
            await connection.QueryFirstOrDefaultAsync<CertificateDbRow>(
                new CommandDefinition(
                    "sp_MoveCertificateStatus",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        return result != null;
    }

    // =========================================================
    // DELETE
    // =========================================================

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0)
            return false;

        using var connection = _database.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add(
            "p_CertificateId",
            id,
            DbType.Int32);

        var rowsAffected =
            await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    "sp_DeleteCertificate",
                    parameters,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));

        return rowsAffected > 0;
    }

    // =========================================================
    // MAP DATABASE RESULT TO DTO
    // =========================================================

    private static CertificateResponseDto MapToDto(
        CertificateDbRow row)
    {
        return new CertificateResponseDto
        {
            CertificateId = row.CertificateId,

            CertificateNumber =
                row.CertificateNumber ?? string.Empty,

            StudentId = row.StudentId,

            AdmissionNo =
                row.AdmissionNo ?? string.Empty,

            StudentName =
                row.StudentName ?? string.Empty,

            GroupId = row.GroupId,

            GroupName = row.GroupName,

            Section = row.Section,

            Board = row.Board,

            AcademicLevel = row.AcademicLevel,

            AcademicYear = row.AcademicYear,

            CertificateType =
                row.CertificateType ?? string.Empty,

            Purpose =
                row.Purpose ?? string.Empty,

            Remarks = row.Remarks,

            Status =
                row.Status ?? string.Empty,

            IssueDate = row.IssueDate,

            GeneratedAt = row.GeneratedAt,

            ReviewedAt = row.ReviewedAt,

            ApprovedAt = row.ApprovedAt,

            IssuedAt = row.IssuedAt,

            IssuedBy = row.IssuedBy,

            Signature = row.Signature,

            IsReissued = row.IsReissued,

            CreatedAt = row.CreatedAt,

            IsActive = row.IsActive
        };
    }

    // =========================================================
    // SEARCH HELPER
    // =========================================================

    private static bool Contains(
        string? value,
        string search)
    {
        return !string.IsNullOrWhiteSpace(value)
            && value.Contains(
                search,
                StringComparison.OrdinalIgnoreCase);
    }

    // =========================================================
    // INTERNAL DAPPER DATABASE MODEL
    // =========================================================

    private sealed class CertificateDbRow
    {
        public int CertificateId { get; set; }

        public string? CertificateNumber { get; set; }

        public int StudentId { get; set; }

        public string? AdmissionNo { get; set; }

        public string? StudentName { get; set; }

        public int GroupId { get; set; }

        public string? GroupName { get; set; }

        public string? Section { get; set; }

        public string? Board { get; set; }

        public string? AcademicLevel { get; set; }

        public string? AcademicYear { get; set; }

        public string? CertificateType { get; set; }

        public string? Purpose { get; set; }

        public string? Remarks { get; set; }

        public string? Status { get; set; }

        public DateTime IssueDate { get; set; }

        public DateTime GeneratedAt { get; set; }

        public DateTime? ReviewedAt { get; set; }

        public DateTime? ApprovedAt { get; set; }

        public DateTime? IssuedAt { get; set; }

        public string? IssuedBy { get; set; }

        public string? Signature { get; set; }

        public bool IsReissued { get; set; }

        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}