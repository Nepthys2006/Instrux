namespace Instrux.Services.DTOs;

public class AssessmentDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public double MaxScore { get; set; }
    public DateTime DateCreated { get; set; }
}
