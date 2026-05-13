using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface ITeacherProfileService
{
    Task<Result<TeacherProfileDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<TeacherProfileDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<TeacherProfileDto>> CreateAsync(CreateTeacherProfileDto dto, CancellationToken ct = default);
    Task<Result<TeacherProfileDto>> UpdateAsync(Guid id, CreateTeacherProfileDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
