using FluentValidation;
using Instrux.Services.DTOs;

namespace Instrux.Services.Validators;

public class CreateGradeValidator : AbstractValidator<CreateGradeDto>
{
    public CreateGradeValidator()
    {
        RuleFor(x => x.AssessmentId).NotEmpty();
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.Score).GreaterThanOrEqualTo(0).When(x => x.Score.HasValue);
    }
}
