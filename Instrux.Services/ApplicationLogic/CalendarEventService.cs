using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

public class CalendarEventService : ICalendarEventService
{
    private readonly IRepository<CalendarEvent> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CalendarEventService(
        IRepository<CalendarEvent> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CalendarEventDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<CalendarEventDto>.Failure($"CalendarEvent {id} not found.");
        return Result<CalendarEventDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<CalendarEventDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct);
        return Result<IReadOnlyList<CalendarEventDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<CalendarEventDto>> CreateAsync(CreateCalendarEventDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<CalendarEventDto>.Success(MapToDto(entity));
    }

    public async Task<Result<CalendarEventDto>> UpdateAsync(Guid id, CreateCalendarEventDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<CalendarEventDto>.Failure($"CalendarEvent {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<CalendarEventDto>.Success(MapToDto(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"CalendarEvent {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static CalendarEventDto MapToDto(CalendarEvent entity) => new()
    {
        Id = entity.Id,
        Title = entity.Title,
        Date = entity.Date,
        TimeRange = entity.TimeRange,
        Category = entity.Category,
        ClassId = entity.ClassId
    };

    private static CalendarEvent MapToEntity(CreateCalendarEventDto dto) => new()
    {
        Title = dto.Title,
        Date = dto.Date,
        TimeRange = dto.TimeRange,
        Category = dto.Category,
        ClassId = dto.ClassId
    };

    private static void ApplyDto(CreateCalendarEventDto dto, CalendarEvent entity)
    {
        entity.Title = dto.Title;
        entity.Date = dto.Date;
        entity.TimeRange = dto.TimeRange;
        entity.Category = dto.Category;
        entity.ClassId = dto.ClassId;
    }
}
