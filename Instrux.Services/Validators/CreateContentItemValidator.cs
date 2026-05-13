using FluentValidation;
using Instrux.Services.DTOs;

namespace Instrux.Services.Validators;

public class CreateContentItemValidator : AbstractValidator<CreateContentItemDto>
{
    public CreateContentItemValidator()
    {
        RuleFor(x => x.ClassId).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Url).NotEmpty().MaximumLength(2048);
        RuleFor(x => x.Type).NotEmpty().MaximumLength(50);
    }
}
