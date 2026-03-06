using FluentValidation;

namespace WCM.API.ApiService.Application.Incidents.GetIncidentById;

public class GetIncidentByIdQueryValidator : AbstractValidator<GetIncidentByIdQuery>
{
    public GetIncidentByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
