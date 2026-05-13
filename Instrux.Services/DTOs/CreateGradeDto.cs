namespace Instrux.Services.DTOs;

public class CreateGradeDto
{
    public Guid AssessmentId { get; set; }
    public Guid StudentId { get; set; }
    public double? Score { get; set; }
}
