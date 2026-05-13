using AutoMapper;
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
    private readonly IMapper _mapper;

    public AssessmentService(
        IRepository<Assessment> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<AssessmentDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, a => a.Class))
            .FirstOrDefault(a => a.Id == id);
        if (entity is null)
            return Result<AssessmentDto>.Failure($"Assessment {id} not found.");
        return Result<AssessmentDto>.Success(_mapper.Map<AssessmentDto>(entity));
    }

    public async Task<Result<IReadOnlyList<AssessmentDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, a => a.Class);
        return Result<IReadOnlyList<AssessmentDto>>.Success(
            _mapper.Map<List<AssessmentDto>>(entities));
    }

    public async Task<Result<AssessmentDto>> CreateAsync(CreateAssessmentDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Assessment>(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<AssessmentDto>.Success(_mapper.Map<AssessmentDto>(entity));
    }

    public async Task<Result<AssessmentDto>> UpdateAsync(Guid id, CreateAssessmentDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<AssessmentDto>.Failure($"Assessment {id} not found.");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<AssessmentDto>.Success(_mapper.Map<AssessmentDto>(entity));
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
}
