using FluentValidation;

namespace WCM.API.ApiService.Application.WasteTypes.UpdateWasteType;

public class UpdateWasteTypeCommandValidator : AbstractValidator<UpdateWasteTypeCommand>
{
    public UpdateWasteTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description is not null);

        RuleFor(x => x.ColorCode)
            .Matches(@"^#[0-9A-Fa-f]{6}$")
            .WithMessage("ColorCode must be a valid hex color (e.g., #FF5733).")
            .When(x => x.ColorCode is not null);
    }
}
