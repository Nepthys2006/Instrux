using AutoMapper;
using Instrux.Domain.Interfaces;
using Instrux.Domain.Models;
using Instrux.Services.Common;
using Instrux.Services.DTOs;
using Instrux.Services.Interfaces;

namespace Instrux.Services.ApplicationLogic;

#pragma warning disable CS8603

public class StudentService : IStudentService
{
    private readonly IRepository<Student> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentService(
        IRepository<Student> repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<Result<StudentDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, s => s.Class))
            .FirstOrDefault(s => s.Id == id);
        if (entity is null)
            return Result<StudentDto>.Failure($"Student {id} not found.");
        return Result<StudentDto>.Success(_mapper.Map<StudentDto>(entity));
    }

    public async Task<Result<IReadOnlyList<StudentDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, s => s.Class);
        return Result<IReadOnlyList<StudentDto>>.Success(
            _mapper.Map<List<StudentDto>>(entities));
    }

    public async Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto, CancellationToken ct = default)
    {
        var entity = _mapper.Map<Student>(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<StudentDto>.Success(_mapper.Map<StudentDto>(entity));
    }

    public async Task<Result<StudentDto>> UpdateAsync(Guid id, CreateStudentDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<StudentDto>.Failure($"Student {id} not found.");

        _mapper.Map(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<StudentDto>.Success(_mapper.Map<StudentDto>(entity));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result.Failure($"Student {id} not found.");

        await _repository.DeleteAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
