public class UpdateFeeStructureDto
{
    public int BoardId { get; set; }
    public int AcademicYearId { get; set; }
    public int GroupId { get; set; }
    public string FeeType { get; set; } = string.Empty;
    public decimal Amount { get; set; }
   
    public DateTime DueDate { get; set; }
    public bool IsActive { get; set; }

}