using FluentValidation;
using Instrux.Services.DTOs;

namespace Instrux.Services.Validators;

public class CreateAttendanceRecordValidator : AbstractValidator<CreateAttendanceRecordDto>
{
    public CreateAttendanceRecordValidator()
    {
        RuleFor(x => x.ClassId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Status).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Note).MaximumLength(500);
    }
}
