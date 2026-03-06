using FluentValidation;

namespace WCM.API.ApiService.Application.Incidents.UpdateIncident;

public class UpdateIncidentCommandValidator : AbstractValidator<UpdateIncidentCommand>
{
    public UpdateIncidentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Priority)
            .IsInEnum();
    }
}
