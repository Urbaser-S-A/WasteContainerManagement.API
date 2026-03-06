using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Containers.DeleteContainer;

namespace WCM.API.Tests.Validators;

public class DeleteContainerCommandValidatorTests
{
    private readonly DeleteContainerCommandValidator _validator = new();

    [Fact]
    public void Should_pass_when_id_is_valid()
    {
        DeleteContainerCommand command = new(Guid.NewGuid());
        TestValidationResult<DeleteContainerCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        DeleteContainerCommand command = new(Guid.Empty);
        TestValidationResult<DeleteContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
