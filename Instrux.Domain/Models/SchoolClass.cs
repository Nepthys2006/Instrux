namespace Instrux.Domain.Models;

public class SchoolClass
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string Term { get; set; } = string.Empty;
    public string ColorHex { get; set; } = "#4F46E5";
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;

    public virtual ICollection<Student> Students { get; private set; } = new List<Student>();
    public virtual ICollection<ContentItem> Contents { get; private set; } = new List<ContentItem>();
    public virtual ICollection<Assessment> Assessments { get; private set; } = new List<Assessment>();
}
