using Instrux.Domain.Interfaces;

namespace Instrux.Domain.Models;

public class TeacherProfile : IAuditableEntity
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string FullName { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
}
