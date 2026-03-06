using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.WasteTypes.UpdateWasteType;

namespace WCM.API.Tests.Validators;

public class UpdateWasteTypeCommandValidatorTests
{
    private readonly UpdateWasteTypeCommandValidator _validator = new();

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        UpdateWasteTypeCommand command = new(Guid.NewGuid(), "Organic", "Organic waste", "#00FF00", true);
        TestValidationResult<UpdateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        UpdateWasteTypeCommand command = new(Guid.Empty, "Organic", null, null, true);
        TestValidationResult<UpdateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        UpdateWasteTypeCommand command = new(Guid.NewGuid(), "", null, null, true);
        TestValidationResult<UpdateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_fail_when_name_exceeds_max_length()
    {
        string name = new('A', 101);
        UpdateWasteTypeCommand command = new(Guid.NewGuid(), name, null, null, true);
        TestValidationResult<UpdateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_fail_when_color_code_is_invalid()
    {
        UpdateWasteTypeCommand command = new(Guid.NewGuid(), "Organic", null, "invalid", true);
        TestValidationResult<UpdateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ColorCode);
    }
}
