namespace Instrux.Domain.Models;

public class Grade
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid AssessmentId { get; set; }
    public Guid StudentId { get; set; }
    public double? Score { get; set; }

    public virtual Assessment? Assessment { get; set; }
    public virtual Student? Student { get; set; }
}
