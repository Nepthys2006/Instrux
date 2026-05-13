namespace Instrux.Services.DTOs;

public class CreateContentItemDto
{
    public Guid ClassId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
