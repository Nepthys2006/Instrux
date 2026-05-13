using AutoMapper;
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
    private readonly IMapper _mapper;

    public TodoItemService(
        IRepository<TodoItem> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TodoItemDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<TodoItemDto>.Failure($"TodoItem {id} not found.");
        return Result<TodoItemDto>.Success(_mapper.Map<TodoItemDto>(entity));
    }

    public async Task<Result<IReadOnlyList<TodoItemDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct);
        return Result<IReadOnlyList<TodoItemDto>>.Success(
            _mapper.Map<List<TodoItemDto>>(entities));
    }

    public async Task<Result<TodoItemDto>> CreateAsync(CreateTodoItemDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<TodoItem>(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<TodoItemDto>.Success(_mapper.Map<TodoItemDto>(entity));
    }

    public async Task<Result<TodoItemDto>> UpdateAsync(Guid id, CreateTodoItemDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<TodoItemDto>.Failure($"TodoItem {id} not found.");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<TodoItemDto>.Success(_mapper.Map<TodoItemDto>(entity));
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
}
