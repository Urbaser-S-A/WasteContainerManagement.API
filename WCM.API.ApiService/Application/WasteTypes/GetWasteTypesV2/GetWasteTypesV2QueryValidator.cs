using FluentValidation;

namespace WCM.API.ApiService.Application.WasteTypes.GetWasteTypesV2;

public class GetWasteTypesV2QueryValidator : AbstractValidator<GetWasteTypesV2Query>
{
    public GetWasteTypesV2QueryValidator()
    {
        // IsActive is optional, no validation rules needed
    }
}
