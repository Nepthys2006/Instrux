using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

#pragma warning disable CS8603

public class GradeService : IGradeService
{
    private readonly IRepository<Grade> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public GradeService(
        IRepository<Grade> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<GradeDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, g => g.Assessment, g => g.Student))
            .FirstOrDefault(g => g.Id == id);
        if (entity is null)
            return Result<GradeDto>.Failure($"Grade {id} not found.");
        return Result<GradeDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<GradeDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, g => g.Assessment, g => g.Student);
        return Result<IReadOnlyList<GradeDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<GradeDto>> CreateAsync(CreateGradeDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<GradeDto>.Success(MapToDto(entity));
    }

    public async Task<Result<GradeDto>> UpdateAsync(Guid id, CreateGradeDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<GradeDto>.Failure($"Grade {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<GradeDto>.Success(MapToDto(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"Grade {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static GradeDto MapToDto(Grade entity) => new()
    {
        Id = entity.Id,
        AssessmentId = entity.AssessmentId,
        AssessmentName = entity.Assessment?.Name ?? string.Empty,
        StudentId = entity.StudentId,
        StudentName = entity.Student?.Name ?? string.Empty,
        Score = entity.Score
    };

    private static Grade MapToEntity(CreateGradeDto dto) => new()
    {
        AssessmentId = dto.AssessmentId,
        StudentId = dto.StudentId,
        Score = dto.Score
    };

    private static void ApplyDto(CreateGradeDto dto, Grade entity)
    {
        entity.AssessmentId = dto.AssessmentId;
        entity.StudentId = dto.StudentId;
        entity.Score = dto.Score;
    }
}
