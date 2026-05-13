using FluentValidation;
using Instrux.Services.DTOs;

namespace Instrux.Services.Validators;

public class CreateTodoItemValidator : AbstractValidator<CreateTodoItemDto>
{
    public CreateTodoItemValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Priority).MaximumLength(20);
    }
}
