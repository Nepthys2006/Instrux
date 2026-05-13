namespace Instrux.Domain.Common;

public interface IDomainEvent
{
    DateTime OccurredAt { get; }
}
