using FluentValidation;

namespace WCM.API.ApiService.Application.Zones.GetZoneById;

public class GetZoneByIdQueryValidator : AbstractValidator<GetZoneByIdQuery>
{
    public GetZoneByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
