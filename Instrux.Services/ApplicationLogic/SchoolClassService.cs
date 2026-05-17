using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

public class SchoolClassService : ISchoolClassService
{
    private readonly IRepository<SchoolClass> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public SchoolClassService(
        IRepository<SchoolClass> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SchoolClassDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, sc => sc.Students);
        var entity = entities.FirstOrDefault(e => e.Id == id);
        if (entity is null)
            return Result<SchoolClassDto>.Failure($"SchoolClass {id} not found.");
        return Result<SchoolClassDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<SchoolClassDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, sc => sc.Students);
        return Result<IReadOnlyList<SchoolClassDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<SchoolClassDto>> CreateAsync(CreateSchoolClassDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<SchoolClassDto>.Success(MapToDto(entity));
    }

    public async Task<Result<SchoolClassDto>> UpdateAsync(Guid id, CreateSchoolClassDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<SchoolClassDto>.Failure($"SchoolClass {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<SchoolClassDto>.Success(MapToDto(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"SchoolClass {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static SchoolClassDto MapToDto(SchoolClass entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Subject = entity.Subject,
        Section = entity.Section,
        Term = entity.Term,
        ColorHex = entity.ColorHex,
        CreatedAt = entity.CreatedAt,
        StudentCount = entity.Students?.Count ?? 0
    };

    private static SchoolClass MapToEntity(CreateSchoolClassDto dto) => new()
    {
        Name = dto.Name,
        Subject = dto.Subject,
        Section = dto.Section,
        Term = dto.Term,
        ColorHex = dto.ColorHex
    };

    private static void ApplyDto(CreateSchoolClassDto dto, SchoolClass entity)
    {
        entity.Name = dto.Name;
        entity.Subject = dto.Subject;
        entity.Section = dto.Section;
        entity.Term = dto.Term;
        entity.ColorHex = dto.ColorHex;
    }
}
