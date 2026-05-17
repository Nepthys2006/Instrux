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

    public AttendanceRecordService(
        IRepository<AttendanceRecord> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AttendanceRecordDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, a => a.Student))
            .FirstOrDefault(a => a.Id == id);
        if (entity is null)
            return Result<AttendanceRecordDto>.Failure($"AttendanceRecord {id} not found.");
        return Result<AttendanceRecordDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<AttendanceRecordDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, a => a.Student);
        return Result<IReadOnlyList<AttendanceRecordDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<AttendanceRecordDto>> CreateAsync(CreateAttendanceRecordDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<AttendanceRecordDto>.Success(MapToDto(entity));
    }

    public async Task<Result<AttendanceRecordDto>> UpdateAsync(Guid id, CreateAttendanceRecordDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<AttendanceRecordDto>.Failure($"AttendanceRecord {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<AttendanceRecordDto>.Success(MapToDto(entity));
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

    private static AttendanceRecordDto MapToDto(AttendanceRecord entity) => new()
    {
        Id = entity.Id,
        ClassId = entity.ClassId,
        StudentId = entity.StudentId,
        StudentName = entity.Student?.Name ?? string.Empty,
        Date = entity.Date,
        Status = entity.Status,
        Note = entity.Note
    };

    private static AttendanceRecord MapToEntity(CreateAttendanceRecordDto dto) => new()
    {
        ClassId = dto.ClassId,
        StudentId = dto.StudentId,
        Date = dto.Date,
        Status = dto.Status,
        Note = dto.Note
    };

    private static void ApplyDto(CreateAttendanceRecordDto dto, AttendanceRecord entity)
    {
        entity.ClassId = dto.ClassId;
        entity.StudentId = dto.StudentId;
        entity.Date = dto.Date;
        entity.Status = dto.Status;
        entity.Note = dto.Note;
    }
}
