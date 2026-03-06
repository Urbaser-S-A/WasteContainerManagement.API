using FluentValidation;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypeById;

public class GetWasteTypeByIdQueryValidator : AbstractValidator<GetWasteTypeByIdQuery>
{
    public GetWasteTypeByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
