using AutoMapper;
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
    private readonly IMapper _mapper;

    public SchoolClassService(
        IRepository<SchoolClass> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<SchoolClassDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, sc => sc.Students);
        var entity = entities.FirstOrDefault(e => e.Id == id);
        if (entity is null)
            return Result<SchoolClassDto>.Failure($"SchoolClass {id} not found.");
        return Result<SchoolClassDto>.Success(_mapper.Map<SchoolClassDto>(entity));
    }

    public async Task<Result<IReadOnlyList<SchoolClassDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, sc => sc.Students);
        return Result<IReadOnlyList<SchoolClassDto>>.Success(
            _mapper.Map<List<SchoolClassDto>>(entities));
    }

    public async Task<Result<SchoolClassDto>> CreateAsync(CreateSchoolClassDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<SchoolClass>(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<SchoolClassDto>.Success(_mapper.Map<SchoolClassDto>(entity));
    }

    public async Task<Result<SchoolClassDto>> UpdateAsync(Guid id, CreateSchoolClassDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<SchoolClassDto>.Failure($"SchoolClass {id} not found.");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<SchoolClassDto>.Success(_mapper.Map<SchoolClassDto>(entity));
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
}
