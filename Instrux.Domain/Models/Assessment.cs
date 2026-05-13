namespace Instrux.Domain.Models;

public class Assessment
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ClassId { get; set; }
    public string Name { get; set; } = string.Empty;
    public double MaxScore { get; set; }
    public DateTime DateCreated { get; private set; } = DateTime.UtcNow;

    public virtual SchoolClass? Class { get; set; }
    public virtual ICollection<Grade> Grades { get; private set; } = new List<Grade>();
}
