using FluentValidation;

namespace WCM.API.ApiService.Application.Zones.DeleteZone;

public class DeleteZoneCommandValidator : AbstractValidator<DeleteZoneCommand>
{
    public DeleteZoneCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
