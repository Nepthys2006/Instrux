namespace Instrux.Domain.Models;

public class AttendanceRecord
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ClassId { get; set; }
    public Guid StudentId { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Note { get; set; }

    public virtual Student? Student { get; set; }
}
