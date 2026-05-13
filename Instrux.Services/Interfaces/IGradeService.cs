using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface IGradeService
{
    Task<Result<GradeDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<GradeDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<GradeDto>> CreateAsync(CreateGradeDto dto, CancellationToken ct = default);
    Task<Result<GradeDto>> UpdateAsync(Guid id, CreateGradeDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
