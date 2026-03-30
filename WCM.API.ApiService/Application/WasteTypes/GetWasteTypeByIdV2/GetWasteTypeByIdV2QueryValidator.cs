using FluentValidation;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypeByIdV2;

public class GetWasteTypeByIdV2QueryValidator : AbstractValidator<GetWasteTypeByIdV2Query>
{
    public GetWasteTypeByIdV2QueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
