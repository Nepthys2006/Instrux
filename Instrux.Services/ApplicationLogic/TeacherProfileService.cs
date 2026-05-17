using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

public class TeacherProfileService : ITeacherProfileService
{
    private readonly IRepository<TeacherProfile> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public TeacherProfileService(
        IRepository<TeacherProfile> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<TeacherProfileDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<TeacherProfileDto>.Failure($"TeacherProfile {id} not found.");
        return Result<TeacherProfileDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<TeacherProfileDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct);
        return Result<IReadOnlyList<TeacherProfileDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<TeacherProfileDto>> CreateAsync(CreateTeacherProfileDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<TeacherProfileDto>.Success(MapToDto(entity));
    }

    public async Task<Result<TeacherProfileDto>> UpdateAsync(Guid id, CreateTeacherProfileDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<TeacherProfileDto>.Failure($"TeacherProfile {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<TeacherProfileDto>.Success(MapToDto(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"TeacherProfile {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static TeacherProfileDto MapToDto(TeacherProfile entity) => new()
    {
        Id = entity.Id,
        FullName = entity.FullName,
        Nickname = entity.Nickname,
        Email = entity.Email,
        CreatedAt = entity.CreatedAt
    };

    private static TeacherProfile MapToEntity(CreateTeacherProfileDto dto) => new()
    {
        FullName = dto.FullName,
        Nickname = dto.Nickname,
        Email = dto.Email
    };

    private static void ApplyDto(CreateTeacherProfileDto dto, TeacherProfile entity)
    {
        entity.FullName = dto.FullName;
        entity.Nickname = dto.Nickname;
        entity.Email = dto.Email;
    }
}
