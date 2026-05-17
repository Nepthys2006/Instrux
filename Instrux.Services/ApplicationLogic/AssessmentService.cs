using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

#pragma warning disable CS8603

public class AssessmentService : IAssessmentService
{
    private readonly IRepository<Assessment> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AssessmentService(
        IRepository<Assessment> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<AssessmentDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, a => a.Class))
            .FirstOrDefault(a => a.Id == id);
        if (entity is null)
            return Result<AssessmentDto>.Failure($"Assessment {id} not found.");
        return Result<AssessmentDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<AssessmentDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, a => a.Class);
        return Result<IReadOnlyList<AssessmentDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<AssessmentDto>> CreateAsync(CreateAssessmentDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<AssessmentDto>.Success(MapToDto(entity));
    }

    public async Task<Result<AssessmentDto>> UpdateAsync(Guid id, CreateAssessmentDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<AssessmentDto>.Failure($"Assessment {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<AssessmentDto>.Success(MapToDto(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"Assessment {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static AssessmentDto MapToDto(Assessment entity) => new()
    {
        Id = entity.Id,
        ClassId = entity.ClassId,
        ClassName = entity.Class?.Name ?? string.Empty,
        Name = entity.Name,
        MaxScore = entity.MaxScore,
        DateCreated = entity.DateCreated
    };

    private static Assessment MapToEntity(CreateAssessmentDto dto) => new()
    {
        ClassId = dto.ClassId,
        Name = dto.Name,
        MaxScore = dto.MaxScore
    };

    private static void ApplyDto(CreateAssessmentDto dto, Assessment entity)
    {
        entity.ClassId = dto.ClassId;
        entity.Name = dto.Name;
        entity.MaxScore = dto.MaxScore;
    }
}
