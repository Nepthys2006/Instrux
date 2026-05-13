using FluentValidation;
using Instrux.Services.DTOs;

namespace Instrux.Services.Validators;

public class CreateSchoolClassValidator : AbstractValidator<CreateSchoolClassDto>
{
    public CreateSchoolClassValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Subject)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Section)
            .MaximumLength(50);

        RuleFor(x => x.Term)
            .MaximumLength(50);

        RuleFor(x => x.ColorHex)
            .NotEmpty()
            .Matches("^#[0-9A-Fa-f]{6}$")
            .WithMessage("ColorHex must be a valid hex color (e.g. #4F46E5)");
    }
}
