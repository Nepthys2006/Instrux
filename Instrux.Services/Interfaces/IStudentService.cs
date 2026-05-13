using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface IStudentService
{
    Task<Result<StudentDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<StudentDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto, CancellationToken ct = default);
    Task<Result<StudentDto>> UpdateAsync(Guid id, CreateStudentDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
