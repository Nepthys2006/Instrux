namespace Instrux.Services.DTOs;

public class CreateCalendarEventDto
{
    public string Title { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string? TimeRange { get; set; }
    public string Category { get; set; } = "General";
    public Guid? ClassId { get; set; }
}
