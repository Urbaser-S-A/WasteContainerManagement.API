using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.WasteTypes.CreateWasteType;

namespace WCM.API.Tests.Validators;

public class CreateWasteTypeCommandValidatorTests
{
    private readonly CreateWasteTypeCommandValidator _validator = new();

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        CreateWasteTypeCommand command = new("Organic", "Organic waste", "#00FF00", true);
        TestValidationResult<CreateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_pass_when_optional_fields_are_null()
    {
        CreateWasteTypeCommand command = new("Organic", null, null, true);
        TestValidationResult<CreateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_fail_when_name_is_empty_or_null(string? name)
    {
        CreateWasteTypeCommand command = new(name!, null, null, true);
        TestValidationResult<CreateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_fail_when_name_exceeds_max_length()
    {
        string name = new('A', 101);
        CreateWasteTypeCommand command = new(name, null, null, true);
        TestValidationResult<CreateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_fail_when_description_exceeds_max_length()
    {
        string description = new('A', 501);
        CreateWasteTypeCommand command = new("Organic", description, null, true);
        TestValidationResult<CreateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Theory]
    [InlineData("FF5733")]
    [InlineData("#GG5733")]
    [InlineData("#FF573")]
    [InlineData("red")]
    public void Should_fail_when_color_code_is_invalid(string colorCode)
    {
        CreateWasteTypeCommand command = new("Organic", null, colorCode, true);
        TestValidationResult<CreateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ColorCode);
    }

    [Theory]
    [InlineData("#FF5733")]
    [InlineData("#000000")]
    [InlineData("#ffffff")]
    public void Should_pass_when_color_code_is_valid(string colorCode)
    {
        CreateWasteTypeCommand command = new("Organic", null, colorCode, true);
        TestValidationResult<CreateWasteTypeCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.ColorCode);
    }
}
