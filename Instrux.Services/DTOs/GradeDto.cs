namespace Instrux.Services.DTOs;

public class GradeDto
{
    public Guid Id { get; set; }
    public Guid AssessmentId { get; set; }
    public string AssessmentName { get; set; } = string.Empty;
    public Guid StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public double? Score { get; set; }
}
