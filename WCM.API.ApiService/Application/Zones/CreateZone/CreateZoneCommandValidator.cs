using FluentValidation;

namespace WCM.API.ApiService.Application.Zones.CreateZone;

public class CreateZoneCommandValidator : AbstractValidator<CreateZoneCommand>
{
    public CreateZoneCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.District)
            .MaximumLength(100)
            .When(x => x.District is not null);

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => x.City is not null);
    }
}
