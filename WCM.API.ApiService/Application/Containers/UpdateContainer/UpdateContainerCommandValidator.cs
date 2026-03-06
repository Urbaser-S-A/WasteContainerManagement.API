using FluentValidation;

namespace WCM.API.ApiService.Application.Containers.UpdateContainer;

public class UpdateContainerCommandValidator : AbstractValidator<UpdateContainerCommand>
{
    public UpdateContainerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.WasteTypeId)
            .NotEmpty()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.ZoneId)
            .NotEmpty()
            .NotEqual(Guid.Empty);

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180);

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .When(x => x.Address is not null);

        RuleFor(x => x.CapacityLiters)
            .GreaterThan(0);

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
