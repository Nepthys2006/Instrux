using FluentValidation;
using Instrux.Services.DTOs;

namespace Instrux.Services.Validators;

public class CreateAssessmentValidator : AbstractValidator<CreateAssessmentDto>
{
    public CreateAssessmentValidator()
    {
        RuleFor(x => x.ClassId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.MaxScore).GreaterThan(0);
    }
}
