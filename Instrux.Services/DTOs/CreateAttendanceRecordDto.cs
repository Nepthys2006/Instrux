namespace Instrux.Services.DTOs;

public class CreateAttendanceRecordDto
{
    public Guid ClassId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }
}
