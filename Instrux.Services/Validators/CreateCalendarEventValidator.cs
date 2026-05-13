using FluentValidation;
using Instrux.Services.DTOs;

namespace Instrux.Services.Validators;

public class CreateCalendarEventValidator : AbstractValidator<CreateCalendarEventDto>
{
    public CreateCalendarEventValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.TimeRange).MaximumLength(50);
        RuleFor(x => x.Category).MaximumLength(50);
    }
}
