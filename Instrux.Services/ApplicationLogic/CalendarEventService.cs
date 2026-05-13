using AutoMapper;
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
    private readonly IMapper _mapper;

    public CalendarEventService(
        IRepository<CalendarEvent> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<CalendarEventDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<CalendarEventDto>.Failure($"CalendarEvent {id} not found.");
        return Result<CalendarEventDto>.Success(_mapper.Map<CalendarEventDto>(entity));
    }

    public async Task<Result<IReadOnlyList<CalendarEventDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct);
        return Result<IReadOnlyList<CalendarEventDto>>.Success(
            _mapper.Map<List<CalendarEventDto>>(entities));
    }

    public async Task<Result<CalendarEventDto>> CreateAsync(CreateCalendarEventDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<CalendarEvent>(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<CalendarEventDto>.Success(_mapper.Map<CalendarEventDto>(entity));
    }

    public async Task<Result<CalendarEventDto>> UpdateAsync(Guid id, CreateCalendarEventDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<CalendarEventDto>.Failure($"CalendarEvent {id} not found.");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<CalendarEventDto>.Success(_mapper.Map<CalendarEventDto>(entity));
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
}
