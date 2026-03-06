using FluentValidation;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;

public class GetWasteTypesQueryValidator : AbstractValidator<GetWasteTypesQuery>
{
    public GetWasteTypesQueryValidator()
    {
        // IsActive is optional, no validation rules needed
    }
}
