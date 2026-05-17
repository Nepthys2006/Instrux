using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

#pragma warning disable CS8603

public class ContentItemService : IContentItemService
{
    private readonly IRepository<ContentItem> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ContentItemService(
        IRepository<ContentItem> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ContentItemDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, c => c.Class))
            .FirstOrDefault(c => c.Id == id);
        if (entity is null)
            return Result<ContentItemDto>.Failure($"ContentItem {id} not found.");
        return Result<ContentItemDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<ContentItemDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, c => c.Class);
        return Result<IReadOnlyList<ContentItemDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<ContentItemDto>> CreateAsync(CreateContentItemDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<ContentItemDto>.Success(MapToDto(entity));
    }

    public async Task<Result<ContentItemDto>> UpdateAsync(Guid id, CreateContentItemDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<ContentItemDto>.Failure($"ContentItem {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<ContentItemDto>.Success(MapToDto(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"ContentItem {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    private static ContentItemDto MapToDto(ContentItem entity) => new()
    {
        Id = entity.Id,
        ClassId = entity.ClassId,
        ClassName = entity.Class?.Name ?? string.Empty,
        Title = entity.Title,
        Url = entity.Url,
        Type = entity.Type,
        CreatedAt = entity.CreatedAt
    };

    private static ContentItem MapToEntity(CreateContentItemDto dto) => new()
    {
        ClassId = dto.ClassId,
        Title = dto.Title,
        Url = dto.Url,
        Type = dto.Type
    };

    private static void ApplyDto(CreateContentItemDto dto, ContentItem entity)
    {
        entity.ClassId = dto.ClassId;
        entity.Title = dto.Title;
        entity.Url = dto.Url;
        entity.Type = dto.Type;
    }
}
