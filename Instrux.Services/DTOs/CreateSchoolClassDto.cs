namespace Instrux.Services.DTOs;

public class CreateSchoolClassDto
{
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#4F46E5";
}
