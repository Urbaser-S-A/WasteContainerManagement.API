using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypeById;

namespace WCM.API.Tests.Validators;

public class GetWasteTypeByIdQueryValidatorTests
{
    private readonly GetWasteTypeByIdQueryValidator _validator = new();

    [Fact]
    public void Should_pass_when_id_is_valid()
    {
        GetWasteTypeByIdQuery query = new(Guid.NewGuid());
        TestValidationResult<GetWasteTypeByIdQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        GetWasteTypeByIdQuery query = new(Guid.Empty);
        TestValidationResult<GetWasteTypeByIdQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
