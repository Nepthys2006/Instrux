namespace Instrux.Domain.Models;

public class Student
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StudentIdentifier { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public virtual SchoolClass? Class { get; set; }
    public virtual ICollection<AttendanceRecord> AttendanceRecords { get; private set; } = new List<AttendanceRecord>();
    public virtual ICollection<Grade> Grades { get; private set; } = new List<Grade>();
}
