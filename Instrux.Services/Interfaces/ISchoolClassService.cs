using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface ISchoolClassService
{
    Task<Result<SchoolClassDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<SchoolClassDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<SchoolClassDto>> CreateAsync(CreateSchoolClassDto dto, CancellationToken ct = default);
    Task<Result<SchoolClassDto>> UpdateAsync(Guid id, CreateSchoolClassDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
