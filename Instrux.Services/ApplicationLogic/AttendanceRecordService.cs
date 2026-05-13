using AutoMapper;
using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

#pragma warning disable CS8603

public class AttendanceRecordService : IAttendanceRecordService
{
    private readonly IRepository<AttendanceRecord> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AttendanceRecordService(
        IRepository<AttendanceRecord> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AttendanceRecordDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, a => a.Student))
            .FirstOrDefault(a => a.Id == id);
        if (entity is null)
            return Result<AttendanceRecordDto>.Failure($"AttendanceRecord {id} not found.");
        return Result<AttendanceRecordDto>.Success(_mapper.Map<AttendanceRecordDto>(entity));
    }

    public async Task<Result<IReadOnlyList<AttendanceRecordDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, a => a.Student);
        return Result<IReadOnlyList<AttendanceRecordDto>>.Success(
            _mapper.Map<List<AttendanceRecordDto>>(entities));
    }

    public async Task<Result<AttendanceRecordDto>> CreateAsync(CreateAttendanceRecordDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<AttendanceRecord>(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<AttendanceRecordDto>.Success(_mapper.Map<AttendanceRecordDto>(entity));
    }

    public async Task<Result<AttendanceRecordDto>> UpdateAsync(Guid id, CreateAttendanceRecordDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<AttendanceRecordDto>.Failure($"AttendanceRecord {id} not found.");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<AttendanceRecordDto>.Success(_mapper.Map<AttendanceRecordDto>(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"AttendanceRecord {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
