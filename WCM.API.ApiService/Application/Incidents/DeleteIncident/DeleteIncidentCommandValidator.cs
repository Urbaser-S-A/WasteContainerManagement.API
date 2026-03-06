using FluentValidation;

namespace WCM.API.ApiService.Application.Incidents.DeleteIncident;

public class DeleteIncidentCommandValidator : AbstractValidator<DeleteIncidentCommand>
{
    public DeleteIncidentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
