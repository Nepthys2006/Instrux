using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface IAssessmentService
{
    Task<Result<AssessmentDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<AssessmentDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<AssessmentDto>> CreateAsync(CreateAssessmentDto dto, CancellationToken ct = default);
    Task<Result<AssessmentDto>> UpdateAsync(Guid id, CreateAssessmentDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
