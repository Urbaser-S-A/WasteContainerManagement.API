using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.WasteTypes.GetWasteTypes;

namespace WCM.API.Tests.Validators;

public class GetWasteTypesQueryValidatorTests
{
    private readonly GetWasteTypesQueryValidator _validator = new();

    [Fact]
    public void Should_pass_when_isActive_is_null()
    {
        GetWasteTypesQuery query = new(null);
        TestValidationResult<GetWasteTypesQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_pass_when_isActive_has_value(bool isActive)
    {
        GetWasteTypesQuery query = new(isActive);
        TestValidationResult<GetWasteTypesQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
