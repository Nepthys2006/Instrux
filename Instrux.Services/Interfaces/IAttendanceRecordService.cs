using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface IAttendanceRecordService
{
    Task<Result<AttendanceRecordDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<AttendanceRecordDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<AttendanceRecordDto>> CreateAsync(CreateAttendanceRecordDto dto, CancellationToken ct = default);
    Task<Result<AttendanceRecordDto>> UpdateAsync(Guid id, CreateAttendanceRecordDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
