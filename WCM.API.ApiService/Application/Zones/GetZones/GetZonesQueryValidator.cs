using FluentValidation;

namespace WCM.API.ApiService.Application.Zones.GetZones;

public class GetZonesQueryValidator : AbstractValidator<GetZonesQuery>
{
    public GetZonesQueryValidator()
    {
        RuleFor(x => x.District)
            .MaximumLength(100)
            .When(x => x.District is not null);

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => x.City is not null);
    }
}
