namespace CollegeManagement.API.DTOs.Certificate;

public class CertificateResponseDto
{
    public int CertificateId { get; set; }

    public string CertificateNumber { get; set; } = string.Empty;

    public int StudentId { get; set; }

    public string AdmissionNo { get; set; } = string.Empty;

    public string StudentName { get; set; } = string.Empty;

    public int GroupId { get; set; }

    public string? GroupName { get; set; }

    public string? Section { get; set; }

    public string? Board { get; set; }

    public string? AcademicLevel { get; set; }

    public string? AcademicYear { get; set; }

    public string CertificateType { get; set; } = string.Empty;

    public string Purpose { get; set; } = string.Empty;

    public string? Remarks { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime IssueDate { get; set; }

    public DateTime GeneratedAt { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime? IssuedAt { get; set; }

    public string? IssuedBy { get; set; }

    public bool IsReissued { get; set; }

    public DateTime CreatedAt { get; set; }

    public bool IsActive { get; set; }

    public string? Signature { get; set; }
}