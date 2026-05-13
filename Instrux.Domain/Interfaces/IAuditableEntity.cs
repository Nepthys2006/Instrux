namespace Instrux.Domain.Interfaces;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; }
}
