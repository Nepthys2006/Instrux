namespace Instrux.Services.DTOs;

public class StudentDto
{
    public Guid Id { get; set; }
    public Guid ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string StudentIdentifier { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
