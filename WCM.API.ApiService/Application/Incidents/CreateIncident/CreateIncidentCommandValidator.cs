using FluentValidation;

namespace WCM.API.ApiService.Application.Incidents.CreateIncident;

public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
{
    public CreateIncidentCommandValidator()
    {
        RuleFor(x => x.ContainerId)
            .NotEmpty()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}
