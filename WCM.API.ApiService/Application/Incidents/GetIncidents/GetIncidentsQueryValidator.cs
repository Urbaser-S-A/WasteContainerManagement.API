using FluentValidation;

namespace WCM.API.ApiService.Application.Incidents.GetIncidents;

public class GetIncidentsQueryValidator : AbstractValidator<GetIncidentsQuery>
{
    public GetIncidentsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
