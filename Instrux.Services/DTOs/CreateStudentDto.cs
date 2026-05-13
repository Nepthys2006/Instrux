namespace Instrux.Services.DTOs;

public class CreateStudentDto
{
    public Guid ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StudentIdentifier { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
