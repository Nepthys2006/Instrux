using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface IContentItemService
{
    Task<Result<ContentItemDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<ContentItemDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<ContentItemDto>> CreateAsync(CreateContentItemDto dto, CancellationToken ct = default);
    Task<Result<ContentItemDto>> UpdateAsync(Guid id, CreateContentItemDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
