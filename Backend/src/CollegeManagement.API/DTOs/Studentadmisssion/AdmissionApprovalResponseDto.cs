namespace CollegeManagement.API.DTOs.StudentAdmission
{
    public class AdmissionApprovalResponseDto
    {
        // =========================================================
        // STUDENT CREATED AFTER APPROVAL
        // =========================================================

        public int StudentId { get; set; }

        // Generated/assigned during admission approval.
        public string? RollNo { get; set; }
    }
}