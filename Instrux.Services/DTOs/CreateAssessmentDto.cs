namespace Instrux.Services.DTOs;

public class CreateAssessmentDto
{
    public Guid ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double MaxScore { get; set; }
}
