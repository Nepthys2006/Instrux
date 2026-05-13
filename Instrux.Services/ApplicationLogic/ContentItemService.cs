using AutoMapper;
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
    private readonly IMapper _mapper;

    public ContentItemService(
        IRepository<ContentItem> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<ContentItemDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, c => c.Class))
            .FirstOrDefault(c => c.Id == id);
        if (entity is null)
            return Result<ContentItemDto>.Failure($"ContentItem {id} not found.");
        return Result<ContentItemDto>.Success(_mapper.Map<ContentItemDto>(entity));
    }

    public async Task<Result<IReadOnlyList<ContentItemDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, c => c.Class);
        return Result<IReadOnlyList<ContentItemDto>>.Success(
            _mapper.Map<List<ContentItemDto>>(entities));
    }

    public async Task<Result<ContentItemDto>> CreateAsync(CreateContentItemDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<ContentItem>(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<ContentItemDto>.Success(_mapper.Map<ContentItemDto>(entity));
    }

    public async Task<Result<ContentItemDto>> UpdateAsync(Guid id, CreateContentItemDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<ContentItemDto>.Failure($"ContentItem {id} not found.");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<ContentItemDto>.Success(_mapper.Map<ContentItemDto>(entity));
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
}
