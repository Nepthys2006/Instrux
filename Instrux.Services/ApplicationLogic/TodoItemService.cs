using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

public class TodoItemService : ITodoItemService
{
    private readonly IRepository<TodoItem> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TodoItemService(
        IRepository<TodoItem> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TodoItemDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<TodoItemDto>.Failure($"TodoItem {id} not found.");
        return Result<TodoItemDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<TodoItemDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct);
        return Result<IReadOnlyList<TodoItemDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<TodoItemDto>> CreateAsync(CreateTodoItemDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<TodoItemDto>.Success(MapToDto(entity));
    }

    public async Task<Result<TodoItemDto>> UpdateAsync(Guid id, CreateTodoItemDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<TodoItemDto>.Failure($"TodoItem {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<TodoItemDto>.Success(MapToDto(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"TodoItem {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static TodoItemDto MapToDto(TodoItem entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Priority = entity.Priority,
        IsCompleted = entity.IsCompleted,
        DueDate = entity.DueDate
    };

    private static TodoItem MapToEntity(CreateTodoItemDto dto) => new()
    {
        Title = dto.Title,
        Priority = dto.Priority,
        DueDate = dto.DueDate
    };

    private static void ApplyDto(CreateTodoItemDto dto, TodoItem entity)
    {
        entity.Title = dto.Title;
        entity.Priority = dto.Priority;
        entity.DueDate = dto.DueDate;
    }
}
