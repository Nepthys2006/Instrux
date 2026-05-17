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

    public StudentService(
        IRepository<Student> repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<StudentDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = (await _repository.GetAllAsync(ct, s => s.Class))
            .FirstOrDefault(s => s.Id == id);
        if (entity is null)
            return Result<StudentDto>.Failure($"Student {id} not found.");
        return Result<StudentDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IReadOnlyList<StudentDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetAllAsync(ct, s => s.Class);
        return Result<IReadOnlyList<StudentDto>>.Success(
            entities.Select(MapToDto).ToList());
    }

    public async Task<Result<StudentDto>> CreateAsync(CreateStudentDto dto, CancellationToken ct = default)
    {
        var entity = MapToEntity(dto);
        await _repository.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<StudentDto>.Success(MapToDto(entity));
    }

    public async Task<Result<StudentDto>> UpdateAsync(Guid id, CreateStudentDto dto, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity is null)
            return Result<StudentDto>.Failure($"Student {id} not found.");

        ApplyDto(dto, entity);
        await _repository.UpdateAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return Result<StudentDto>.Success(MapToDto(entity));
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

    private static StudentDto MapToDto(Student entity) => new()
    {
        Id = entity.Id,
        ClassId = entity.ClassId,
        ClassName = entity.Class?.Name ?? string.Empty,
        Name = entity.Name,
        StudentIdentifier = entity.StudentIdentifier,
        Email = entity.Email
    };

    private static Student MapToEntity(CreateStudentDto dto) => new()
    {
        ClassId = dto.ClassId,
        Name = dto.Name,
        StudentIdentifier = dto.StudentIdentifier,
        Email = dto.Email
    };

    private static void ApplyDto(CreateStudentDto dto, Student entity)
    {
        entity.ClassId = dto.ClassId;
        entity.Name = dto.Name;
        entity.StudentIdentifier = dto.StudentIdentifier;
        entity.Email = dto.Email;
    }
}
