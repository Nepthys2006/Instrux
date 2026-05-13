using AutoMapper;
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
    private readonly IMapper _mapper;

    public TeacherProfileService(
        IRepository<TeacherProfile> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<TeacherProfileDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<TeacherProfileDto>.Failure($"TeacherProfile {id} not found.");
        return Result<TeacherProfileDto>.Success(_mapper.Map<TeacherProfileDto>(entity));
    }

    public async Task<Result<IReadOnlyList<TeacherProfileDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct);
        return Result<IReadOnlyList<TeacherProfileDto>>.Success(
            _mapper.Map<List<TeacherProfileDto>>(entities));
    }

    public async Task<Result<TeacherProfileDto>> CreateAsync(CreateTeacherProfileDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<TeacherProfile>(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<TeacherProfileDto>.Success(_mapper.Map<TeacherProfileDto>(entity));
    }

    public async Task<Result<TeacherProfileDto>> UpdateAsync(Guid id, CreateTeacherProfileDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<TeacherProfileDto>.Failure($"TeacherProfile {id} not found.");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<TeacherProfileDto>.Success(_mapper.Map<TeacherProfileDto>(entity));
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
}
