using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Containers.GetContainerById;

namespace WCM.API.Tests.Validators;

public class GetContainerByIdQueryValidatorTests
{
    private readonly GetContainerByIdQueryValidator _validator = new();

    [Fact]
    public void Should_pass_when_id_is_valid()
    {
        GetContainerByIdQuery query = new(Guid.NewGuid());
        TestValidationResult<GetContainerByIdQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        GetContainerByIdQuery query = new(Guid.Empty);
        TestValidationResult<GetContainerByIdQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
