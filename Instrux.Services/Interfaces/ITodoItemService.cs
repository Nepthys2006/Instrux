using Instrux.Services.Common;
using Instrux.Services.DTOs;

namespace Instrux.Services.Interfaces;

public interface ITodoItemService
{
    Task<Result<TodoItemDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<IReadOnlyList<TodoItemDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<TodoItemDto>> CreateAsync(CreateTodoItemDto dto, CancellationToken ct = default);
    Task<Result<TodoItemDto>> UpdateAsync(Guid id, CreateTodoItemDto dto, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
}
