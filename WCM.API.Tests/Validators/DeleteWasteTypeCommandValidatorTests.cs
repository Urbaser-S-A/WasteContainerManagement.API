using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.WasteTypes.DeleteWasteType;

namespace WCM.API.Tests.Validators;

public class DeleteWasteTypeCommandValidatorTests
{
    private readonly DeleteWasteTypeCommandValidator _validator = new();

    [Fact]
    public void Should_pass_when_id_is_valid()
    {
        DeleteWasteTypeCommand command = new(Guid.NewGuid());
        TestValidationResult<DeleteWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        DeleteWasteTypeCommand command = new(Guid.Empty);
        TestValidationResult<DeleteWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
