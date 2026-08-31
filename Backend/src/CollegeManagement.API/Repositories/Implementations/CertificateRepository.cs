using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using CollegeManagement.API.Data;
using CollegeManagement.API.DTOs.Certificate;
using CollegeManagement.API.Repositories.Interfaces;

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
        string? search = null,
        string? status = null,
        string? certificateType = null,
        CancellationToken ct = default)
    {
        using var connection = _database.CreateConnection();

        // Safe query with joins to ensure Student Name, Group, Academic Year are ALWAYS populated from StudentAdmissions / Students
        var sql = @"
            SELECT 
                c.*,
                COALESCE(c.AdmissionNo, sa.AdmissionNo, s.AdmissionNo, '') AS S_AdmissionNo,
                COALESCE(c.StudentName, NULLIF(TRIM(CONCAT(sa.FirstName, ' ', COALESCE(sa.LastName, ''))), ''), s.StudentName, '') AS S_StudentName,
                COALESCE(c.GroupName, g.GroupName, '') AS S_GroupName,
                COALESCE(c.AcademicLevel, al.LevelName, s.AcademicLevel, '1st Year') AS S_AcademicLevel,
                COALESCE(c.AcademicYear, ay.AcademicYearName, '') AS S_AcademicYear
            FROM `certificates` c
            LEFT JOIN `StudentAdmissions` sa ON (TRIM(sa.AdmissionNo) = TRIM(c.AdmissionNo) OR sa.AdmissionId = c.StudentId)
            LEFT JOIN `Students` s ON s.StudentId = c.StudentId OR TRIM(s.AdmissionNo) = TRIM(c.AdmissionNo)
            LEFT JOIN `Groups` g ON g.GroupId = COALESCE(sa.GroupId, s.GroupId)
            LEFT JOIN `AcademicYears` ay ON ay.AcademicYearId = COALESCE(sa.AcademicYearId, s.AcademicYearId)
            LEFT JOIN `AcademicLevels` al ON al.AcademicLevelId = COALESCE(sa.AcademicLevelId, s.AcademicLevelId)
            ORDER BY 1 DESC;";

        try
        {
            var rawRows = await connection.QueryAsync<dynamic>(new CommandDefinition(sql, cancellationToken: ct));
            var dtos = rawRows.Select(MapDynamicToDto).ToList();

            return dtos.Where(c =>
            {
                if (c == null) return false;

                if (!string.IsNullOrWhiteSpace(status) && status != "All" && status != "All Status")
                {
                    if (!string.Equals(c.Status, status, StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                if (!string.IsNullOrWhiteSpace(certificateType) && certificateType != "All")
                {
                    if (!string.Equals(c.CertificateType, certificateType, StringComparison.OrdinalIgnoreCase))
                        return false;
                }

                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Trim();
                    var match = (c.CertificateNumber?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (c.AdmissionNo?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (c.StudentName?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (c.CertificateType?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                (c.Purpose?.Contains(s, StringComparison.OrdinalIgnoreCase) ?? false);
                    if (!match) return false;
                }

                return true;
            }).ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetAllAsync Fallback Error: {ex.Message}");
            return new List<CertificateResponseDto>();
        }
    }

    // =========================================================
    // GET BY ID
    // =========================================================
    public async Task<CertificateResponseDto?> GetByIdAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0) return null;

        using var connection = _database.CreateConnection();

        var cols = await GetCertificateTableColumnsAsync(connection);
        var pk = cols.Contains("CertificateId") ? "c.CertificateId" : "c.Id";

        var sql = $@"
            SELECT 
                c.*,
                COALESCE(c.AdmissionNo, sa.AdmissionNo, s.AdmissionNo, '') AS S_AdmissionNo,
                COALESCE(c.StudentName, NULLIF(TRIM(CONCAT(sa.FirstName, ' ', COALESCE(sa.LastName, ''))), ''), s.StudentName, '') AS S_StudentName,
                COALESCE(c.GroupName, g.GroupName, '') AS S_GroupName,
                COALESCE(c.AcademicLevel, al.LevelName, s.AcademicLevel, '1st Year') AS S_AcademicLevel,
                COALESCE(c.AcademicYear, ay.AcademicYearName, '') AS S_AcademicYear
            FROM `certificates` c
            LEFT JOIN `StudentAdmissions` sa ON (TRIM(sa.AdmissionNo) = TRIM(c.AdmissionNo) OR sa.AdmissionId = c.StudentId)
            LEFT JOIN `Students` s ON s.StudentId = c.StudentId OR TRIM(s.AdmissionNo) = TRIM(c.AdmissionNo)
            LEFT JOIN `Groups` g ON g.GroupId = COALESCE(sa.GroupId, s.GroupId)
            LEFT JOIN `AcademicYears` ay ON ay.AcademicYearId = COALESCE(sa.AcademicYearId, s.AcademicYearId)
            LEFT JOIN `AcademicLevels` al ON al.AcademicLevelId = COALESCE(sa.AcademicLevelId, s.AcademicLevelId)
            WHERE {pk} = @id
            LIMIT 1;";

        try
        {
            var row = await connection.QueryFirstOrDefaultAsync<dynamic>(
                new CommandDefinition(sql, new { id }, cancellationToken: ct));

            return row == null ? null : MapDynamicToDto(row);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetByIdAsync Error: {ex.Message}");
            return null;
        }
    }

    // =========================================================
    // GET WORKFLOW STATS
    // =========================================================
    public async Task<CertificateWorkflowStatsDto> GetWorkflowStatsAsync(
        CancellationToken ct = default)
    {
        var all = await GetAllAsync(null, null, null, ct);

        return new CertificateWorkflowStatsDto
        {
            TotalCount = all.Count,
            GeneratedCount = all.Count(c => string.Equals(c.Status, "Generated", StringComparison.OrdinalIgnoreCase) || string.Equals(c.Status, "Active", StringComparison.OrdinalIgnoreCase)),
            ReviewedCount = all.Count(c => string.Equals(c.Status, "Reviewed", StringComparison.OrdinalIgnoreCase)),
            ApprovedCount = all.Count(c => string.Equals(c.Status, "Approved", StringComparison.OrdinalIgnoreCase)),
            IssuedCount = all.Count(c => string.Equals(c.Status, "Issued", StringComparison.OrdinalIgnoreCase)),
            CancelledCount = all.Count(c => string.Equals(c.Status, "Cancelled", StringComparison.OrdinalIgnoreCase) || string.Equals(c.Status, "Deleted", StringComparison.OrdinalIgnoreCase) || !c.IsActive)
        };
    }

    // =========================================================
    // GET STUDENTS DROPDOWN
    // =========================================================
    public async Task<IReadOnlyList<StudentCertificateDropdownDto>> GetStudentsDropdownAsync(
        CancellationToken ct = default)
    {
        using var connection = _database.CreateConnection();

        var sql = @"
            SELECT 
                COALESCE(sa.AdmissionId, s.StudentId) AS StudentId,
                COALESCE(sa.AdmissionNo, s.AdmissionNo) AS AdmissionNo,
                COALESCE(sa.RollNo, s.RollNo, '') AS RollNo,
                COALESCE(NULLIF(TRIM(CONCAT(sa.FirstName, ' ', COALESCE(sa.LastName, ''))), ''), s.StudentName, '') AS StudentName,
                COALESCE(g.GroupName, '') AS GroupName,
                COALESCE(ay.AcademicYearName, '') AS AcademicYear,
                COALESCE(al.LevelName, s.AcademicLevel, '1st Year') AS AcademicLevel,
                COALESCE(sec.SectionName, '') AS Section
            FROM `StudentAdmissions` sa
            LEFT JOIN `Students` s ON TRIM(s.AdmissionNo) = TRIM(sa.AdmissionNo)
            LEFT JOIN `Groups` g ON g.GroupId = COALESCE(sa.GroupId, s.GroupId)
            LEFT JOIN `AcademicYears` ay ON ay.AcademicYearId = COALESCE(sa.AcademicYearId, s.AcademicYearId)
            LEFT JOIN `AcademicLevels` al ON al.AcademicLevelId = COALESCE(sa.AcademicLevelId, s.AcademicLevelId)
            LEFT JOIN `Sections` sec ON sec.SectionId = COALESCE(sa.SectionId, s.SectionId)
            WHERE (sa.IsActive = 1 OR sa.IsActive IS NULL)
              AND (sa.AdmissionNo IS NOT NULL AND sa.AdmissionNo <> '')
            ORDER BY StudentName ASC;";

        try
        {
            var list = await connection.QueryAsync<StudentCertificateDropdownDto>(
                new CommandDefinition(sql, cancellationToken: ct));

            if (!list.Any())
            {
                // Fallback to Students table if StudentAdmissions is completely empty
                var fallbackSql = @"
                    SELECT 
                        s.StudentId, s.AdmissionNo, s.RollNo, s.StudentName,
                        COALESCE(g.GroupName, '') AS GroupName,
                        COALESCE(ay.AcademicYearName, '') AS AcademicYear,
                        COALESCE(s.AcademicLevel, '1st Year') AS AcademicLevel,
                        COALESCE(sec.SectionName, '') AS Section
                    FROM `Students` s
                    LEFT JOIN `Groups` g ON g.GroupId = s.GroupId
                    LEFT JOIN `AcademicYears` ay ON ay.AcademicYearId = s.AcademicYearId
                    LEFT JOIN `Sections` sec ON sec.SectionId = s.SectionId
                    WHERE (s.IsActive = 1 OR s.IsActive IS NULL)
                      AND (s.AdmissionNo IS NOT NULL AND s.AdmissionNo <> '')
                    ORDER BY s.StudentName ASC;";

                list = await connection.QueryAsync<StudentCertificateDropdownDto>(
                    new CommandDefinition(fallbackSql, cancellationToken: ct));
            }

            return list.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetStudentsDropdownAsync Error: {ex.Message}");
            return new List<StudentCertificateDropdownDto>();
        }
    }

    // =========================================================
    // GENERATE CERTIFICATE
    // =========================================================
    public async Task<CertificateResponseDto?> GenerateAsync(
        GenerateCertificateRequestDto request,
        CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        using var connection = _database.CreateConnection();

        var requestDate = request.RequestDate ?? DateTime.UtcNow;

        // Fetch student details from Students / StudentAdmissions
        var studentSql = @"
            SELECT 
                COALESCE(s.StudentId, sa.AdmissionId, 0) AS StudentId,
                COALESCE(NULLIF(TRIM(CONCAT(sa.FirstName, ' ', COALESCE(sa.LastName, ''))), ''), s.StudentName, @admissionNo) AS StudentName,
                COALESCE(g.GroupName, '') AS GroupName,
                COALESCE(al.LevelName, s.AcademicLevel, '1st Year') AS AcademicLevel,
                COALESCE(ay.AcademicYearName, '') AS AcademicYear
            FROM `Students` s
            LEFT JOIN `StudentAdmissions` sa ON (TRIM(sa.AdmissionNo) = TRIM(s.AdmissionNo) OR sa.AdmissionId = s.StudentId)
            LEFT JOIN `Groups` g ON g.GroupId = COALESCE(sa.GroupId, s.GroupId)
            LEFT JOIN `AcademicYears` ay ON ay.AcademicYearId = COALESCE(sa.AcademicYearId, s.AcademicYearId)
            LEFT JOIN `AcademicLevels` al ON al.AcademicLevelId = COALESCE(sa.AcademicLevelId, s.AcademicLevelId)
            WHERE TRIM(s.AdmissionNo) = TRIM(@admissionNo) OR TRIM(sa.AdmissionNo) = TRIM(@admissionNo)
            LIMIT 1;";

        var student = await connection.QueryFirstOrDefaultAsync<dynamic>(
            new CommandDefinition(studentSql, new { admissionNo = request.AdmissionNo.Trim() }, cancellationToken: ct));

        // If not found in Students, try StudentAdmissions directly
        if (student == null)
        {
            var saSql = @"
                SELECT 
                    sa.AdmissionId AS StudentId,
                    COALESCE(NULLIF(TRIM(CONCAT(sa.FirstName, ' ', COALESCE(sa.LastName, ''))), ''), @admissionNo) AS StudentName,
                    COALESCE(g.GroupName, '') AS GroupName,
                    COALESCE(al.LevelName, '1st Year') AS AcademicLevel,
                    COALESCE(ay.AcademicYearName, '') AS AcademicYear
                FROM `StudentAdmissions` sa
                LEFT JOIN `Groups` g ON g.GroupId = sa.GroupId
                LEFT JOIN `AcademicYears` ay ON ay.AcademicYearId = sa.AcademicYearId
                LEFT JOIN `AcademicLevels` al ON al.AcademicLevelId = sa.AcademicLevelId
                WHERE TRIM(sa.AdmissionNo) = TRIM(@admissionNo)
                LIMIT 1;";

            student = await connection.QueryFirstOrDefaultAsync<dynamic>(
                new CommandDefinition(saSql, new { admissionNo = request.AdmissionNo.Trim() }, cancellationToken: ct));
        }

        string studentName = student != null && !string.IsNullOrWhiteSpace((string?)student.StudentName) 
            ? (string)student.StudentName 
            : request.AdmissionNo.Trim();
        string groupName = student != null && !string.IsNullOrWhiteSpace((string?)student.GroupName) ? (string)student.GroupName : "";
        string academicLevel = student != null && !string.IsNullOrWhiteSpace((string?)student.AcademicLevel) ? (string)student.AcademicLevel : "1st Year";
        string academicYear = student != null && !string.IsNullOrWhiteSpace((string?)student.AcademicYear) ? (string)student.AcademicYear : $"{DateTime.UtcNow.Year}-{DateTime.UtcNow.Year + 1}";
        int studentId = student != null && (int)student.StudentId > 0 ? (int)student.StudentId : 1;

        var yearNum = DateTime.UtcNow.Year.ToString();
        var certPrefix = request.CertificateType switch
        {
            "Bonafide Certificate" => "BON",
            "Study Certificate" => "STU",
            "Conduct Certificate" => "CND",
            "Transfer Certificate" => "TC",
            "Transfer Certificate (TC)" => "TC",
            _ => "CERT"
        };

        var certNumber = $"{certPrefix}-{yearNum}{DateTime.UtcNow:MMdd}-{Random.Shared.Next(100000, 999999)}";

        // Check which columns exist in `certificates` table
        var tableCols = (await connection.QueryAsync<string>(@"
            SELECT COLUMN_NAME FROM information_schema.columns 
            WHERE table_schema = DATABASE() AND table_name = 'certificates';")).ToHashSet(StringComparer.OrdinalIgnoreCase);

        int newId = 0;

        if (tableCols.Contains("CertificateNumber") && tableCols.Contains("StudentName"))
        {
            var insertSql = @"
                INSERT INTO `certificates` (
                    CertificateNumber, StudentId, AdmissionNo, StudentName, GroupName,
                    AcademicLevel, AcademicYear, CertificateType, Purpose, RequestDate,
                    IssueDate, Remarks, Status, GeneratedAt, IsActive, CreatedAt
                ) VALUES (
                    @certNumber, @studentId, @admissionNo, @studentName, @groupName,
                    @academicLevel, @academicYear, @certificateType, @purpose, @requestDate,
                    @requestDate, @remarks, 'Generated', UTC_TIMESTAMP(), 1, UTC_TIMESTAMP()
                );
                SELECT LAST_INSERT_ID();";

            newId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(insertSql, new
                {
                    certNumber,
                    studentId,
                    admissionNo = request.AdmissionNo.Trim(),
                    studentName,
                    groupName,
                    academicLevel,
                    academicYear,
                    certificateType = request.CertificateType.Trim(),
                    purpose = request.Purpose.Trim(),
                    requestDate,
                    remarks = request.Remarks?.Trim()
                }, cancellationToken: ct));
        }
        else
        {
            // Legacy table structure fallback
            var insertLegacySql = @"
                INSERT INTO `certificates` (
                    StudentId, CertificateNo, CertificateType, Purpose, IssueDate, Remarks, Status, CreatedAt, IsVerified
                ) VALUES (
                    @studentId, @certNumber, @certificateType, @purpose, @requestDate, @remarks, 'Generated', UTC_TIMESTAMP(), 1
                );
                SELECT LAST_INSERT_ID();";

            newId = await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(insertLegacySql, new
                {
                    studentId = (int)student.StudentId,
                    certNumber,
                    certificateType = request.CertificateType.Trim(),
                    purpose = request.Purpose.Trim(),
                    requestDate,
                    remarks = request.Remarks?.Trim()
                }, cancellationToken: ct));
        }

        return await GetByIdAsync(newId, ct);
    }

    public async Task<CertificateResponseDto?> GenerateAsync(
        GenerateCertificateDto request,
        CancellationToken ct = default)
    {
        return await GenerateAsync(new GenerateCertificateRequestDto
        {
            AdmissionNo = request.AdmissionNo,
            CertificateType = request.CertificateType,
            Purpose = request.Purpose,
            RequestDate = request.IssueDate,
            Remarks = request.Remarks
        }, ct);
    }

    public async Task<IReadOnlyList<CertificateResponseDto>> GetHistoryAsync(
        string? admissionNo,
        CancellationToken ct = default)
    {
        return await GetAllAsync(admissionNo, null, null, ct);
    }

    public async Task<CertificateResponseDto?> VerifyAsync(
        string certificateNo,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(certificateNo))
            return null;

        var all = await GetAllAsync(null, null, null, ct);
        return all.FirstOrDefault(c => string.Equals(c.CertificateNumber?.Trim(), certificateNo.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task<CertificateResponseDto?> ReissueAsync(
        ReissueCertificateDto request,
        CancellationToken ct = default)
    {
        return await GenerateAsync(new GenerateCertificateRequestDto
        {
            AdmissionNo = request.AdmissionNo,
            CertificateType = request.CertificateType,
            Purpose = request.Purpose,
            RequestDate = request.RequestDate,
            Remarks = $"[Reissue] {request.Remarks}"
        }, ct);
    }

    public async Task<CertificateResponseDto?> UpdateByAdmissionNoAsync(
        UpdateCertificateDto request,
        CancellationToken ct = default)
    {
        using var connection = _database.CreateConnection();

        var sql = @"
            UPDATE `certificates`
            SET 
                CertificateType = COALESCE(NULLIF(@type, ''), CertificateType),
                Purpose = COALESCE(NULLIF(@purpose, ''), Purpose),
                IssueDate = @issueDate,
                Remarks = @remarks
            WHERE StudentId = (SELECT StudentId FROM `Students` WHERE AdmissionNo = @admissionNo LIMIT 1)
            ORDER BY 1 DESC
            LIMIT 1;";

        await connection.ExecuteAsync(new CommandDefinition(sql, new
        {
            type = request.CertificateType?.Trim(),
            purpose = request.Purpose?.Trim(),
            issueDate = request.IssueDate,
            remarks = request.Remarks?.Trim(),
            admissionNo = request.AdmissionNo.Trim()
        }, cancellationToken: ct));

        var all = await GetAllAsync(request.AdmissionNo.Trim(), null, null, ct);
        return all.FirstOrDefault();
    }

    public async Task<bool> MoveStatusAsync(
        int id,
        string status,
        string? issuedBy = null,
        CancellationToken ct = default)
    {
        if (id <= 0 || string.IsNullOrWhiteSpace(status))
            return false;

        using var connection = _database.CreateConnection();

        try
        {
            var cols = await GetCertificateTableColumnsAsync(connection);
            var pk = cols.Contains("CertificateId") ? "CertificateId" : "Id";

            var sql = $"UPDATE `certificates` SET Status = @status WHERE {pk} = @id;";

            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { id, status }, cancellationToken: ct));
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MoveStatusAsync Error: {ex.Message}");
            return false;
        }
    }

    public async Task<int> BulkApproveAsync(
        string approvedBy,
        CancellationToken ct = default)
    {
        using var connection = _database.CreateConnection();

        try
        {
            var sql = "UPDATE `certificates` SET Status = 'Approved' WHERE Status = 'Reviewed';";
            return await connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: ct));
        }
        catch
        {
            return 0;
        }
    }

    public async Task<int> BulkIssueAsync(
        string issuedBy,
        CancellationToken ct = default)
    {
        using var connection = _database.CreateConnection();

        try
        {
            var sql = "UPDATE `certificates` SET Status = 'Issued' WHERE Status = 'Approved';";
            return await connection.ExecuteAsync(new CommandDefinition(sql, cancellationToken: ct));
        }
        catch
        {
            return 0;
        }
    }

    public async Task<bool> CancelAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0) return false;

        using var connection = _database.CreateConnection();

        try
        {
            var cols = await GetCertificateTableColumnsAsync(connection);
            var pk = cols.Contains("CertificateId") ? "CertificateId" : "Id";

            var sql = $"UPDATE `certificates` SET Status = 'Cancelled' WHERE {pk} = @id;";
            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: ct));
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CancelAsync Error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken ct = default)
    {
        if (id <= 0) return false;

        using var connection = _database.CreateConnection();

        try
        {
            var cols = await GetCertificateTableColumnsAsync(connection);
            var pk = cols.Contains("CertificateId") ? "CertificateId" : "Id";

            var sql = $"DELETE FROM `certificates` WHERE {pk} = @id;";
            var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { id }, cancellationToken: ct));
            return affected > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeleteAsync Error: {ex.Message}");
            return false;
        }
    }

    private static async Task<HashSet<string>> GetCertificateTableColumnsAsync(IDbConnection connection)
    {
        try
        {
            var cols = await connection.QueryAsync<string>(@"
                SELECT COLUMN_NAME FROM information_schema.columns 
                WHERE table_schema = DATABASE() AND table_name = 'certificates';");
            return cols.ToHashSet(StringComparer.OrdinalIgnoreCase);
        }
        catch
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Id", "CertificateNo", "Status" };
        }
    }

    // =========================================================
    // DYNAMIC ROW MAPPER (Handles both Legacy & New DB columns)
    // =========================================================
    private static CertificateResponseDto MapDynamicToDto(dynamic row)
    {
        var dict = (IDictionary<string, object>)row;

        int id = 0;
        if (dict.ContainsKey("CertificateId") && dict["CertificateId"] != null)
            id = Convert.ToInt32(dict["CertificateId"]);
        else if (dict.ContainsKey("Id") && dict["Id"] != null)
            id = Convert.ToInt32(dict["Id"]);

        string certNo = "";
        if (dict.ContainsKey("CertificateNumber") && dict["CertificateNumber"] != null)
            certNo = dict["CertificateNumber"].ToString()!;
        else if (dict.ContainsKey("CertificateNo") && dict["CertificateNo"] != null)
            certNo = dict["CertificateNo"].ToString()!;

        int studentId = 0;
        if (dict.ContainsKey("StudentId") && dict["StudentId"] != null)
            studentId = Convert.ToInt32(dict["StudentId"]);

        string admissionNo = "";
        if (dict.ContainsKey("AdmissionNo") && dict["AdmissionNo"] != null && !string.IsNullOrWhiteSpace(dict["AdmissionNo"].ToString()))
            admissionNo = dict["AdmissionNo"].ToString()!;
        else if (dict.ContainsKey("S_AdmissionNo") && dict["S_AdmissionNo"] != null)
            admissionNo = dict["S_AdmissionNo"].ToString()!;

        string studentName = "";
        if (dict.ContainsKey("StudentName") && dict["StudentName"] != null && !string.IsNullOrWhiteSpace(dict["StudentName"].ToString()))
            studentName = dict["StudentName"].ToString()!;
        else if (dict.ContainsKey("S_StudentName") && dict["S_StudentName"] != null)
            studentName = dict["S_StudentName"].ToString()!;

        string groupName = "";
        if (dict.ContainsKey("GroupName") && dict["GroupName"] != null && !string.IsNullOrWhiteSpace(dict["GroupName"].ToString()))
            groupName = dict["GroupName"].ToString()!;
        else if (dict.ContainsKey("S_GroupName") && dict["S_GroupName"] != null)
            groupName = dict["S_GroupName"].ToString()!;

        string academicLevel = "";
        if (dict.ContainsKey("AcademicLevel") && dict["AcademicLevel"] != null && !string.IsNullOrWhiteSpace(dict["AcademicLevel"].ToString()))
            academicLevel = dict["AcademicLevel"].ToString()!;
        else if (dict.ContainsKey("S_AcademicLevel") && dict["S_AcademicLevel"] != null)
            academicLevel = dict["S_AcademicLevel"].ToString()!;

        string academicYear = "";
        if (dict.ContainsKey("AcademicYear") && dict["AcademicYear"] != null && !string.IsNullOrWhiteSpace(dict["AcademicYear"].ToString()))
            academicYear = dict["AcademicYear"].ToString()!;
        else if (dict.ContainsKey("S_AcademicYear") && dict["S_AcademicYear"] != null)
            academicYear = dict["S_AcademicYear"].ToString()!;

        string certType = dict.ContainsKey("CertificateType") && dict["CertificateType"] != null ? dict["CertificateType"].ToString()! : "Certificate";
        string purpose = dict.ContainsKey("Purpose") && dict["Purpose"] != null ? dict["Purpose"].ToString()! : "";
        string remarks = dict.ContainsKey("Remarks") && dict["Remarks"] != null ? dict["Remarks"].ToString()! : "";
        string status = dict.ContainsKey("Status") && dict["Status"] != null ? dict["Status"].ToString()! : "Generated";
        if (status.Equals("Active", StringComparison.OrdinalIgnoreCase)) status = "Generated";

        DateTime reqDate = DateTime.UtcNow;
        if (dict.ContainsKey("RequestDate") && dict["RequestDate"] != null && dict["RequestDate"] is DateTime rdt)
            reqDate = rdt;
        else if (dict.ContainsKey("IssueDate") && dict["IssueDate"] != null && dict["IssueDate"] is DateTime idt)
            reqDate = idt;
        else if (dict.ContainsKey("CreatedAt") && dict["CreatedAt"] != null && dict["CreatedAt"] is DateTime cdt)
            reqDate = cdt;

        DateTime issDate = reqDate;
        if (dict.ContainsKey("IssueDate") && dict["IssueDate"] != null && dict["IssueDate"] is DateTime idt2)
            issDate = idt2;

        return new CertificateResponseDto
        {
            CertificateId = id,
            CertificateNumber = certNo,
            StudentId = studentId,
            AdmissionNo = admissionNo,
            StudentName = studentName,
            GroupName = groupName,
            AcademicLevel = !string.IsNullOrWhiteSpace(academicLevel) ? academicLevel : "1st Year",
            AcademicYear = academicYear,
            CertificateType = certType,
            Purpose = purpose,
            Remarks = remarks,
            Status = status,
            RequestDate = reqDate,
            IssueDate = issDate,
            GeneratedAt = reqDate,
            IsActive = !status.Equals("Cancelled", StringComparison.OrdinalIgnoreCase) && !status.Equals("Deleted", StringComparison.OrdinalIgnoreCase)
        };
    }
}