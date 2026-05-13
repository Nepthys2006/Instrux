using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface ICalendarEventService
{
    Task<Result<CalendarEventDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<CalendarEventDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<CalendarEventDto>> CreateAsync(CreateCalendarEventDto dto, CancellationToken ct = default);
    Task<Result<CalendarEventDto>> UpdateAsync(Guid id, CreateCalendarEventDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
