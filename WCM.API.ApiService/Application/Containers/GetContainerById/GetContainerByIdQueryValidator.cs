using FluentValidation;

namespace WCM.API.ApiService.Application.Containers.GetContainerById;

public class GetContainerByIdQueryValidator : AbstractValidator<GetContainerByIdQuery>
{
    public GetContainerByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
