using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Zones.CreateZone;

namespace WCM.API.Tests.Validators;

public class CreateZoneCommandValidatorTests
{
    private readonly CreateZoneCommandValidator _validator = new();

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        CreateZoneCommand command = new("Zone A", "Centro", "Madrid", true);
        TestValidationResult<CreateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_pass_when_optional_fields_are_null()
    {
        CreateZoneCommand command = new("Zone A", null, null, true);
        TestValidationResult<CreateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_fail_when_name_is_empty_or_null(string? name)
    {
        CreateZoneCommand command = new(name!, null, null, true);
        TestValidationResult<CreateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_fail_when_name_exceeds_max_length()
    {
        string name = new('A', 101);
        CreateZoneCommand command = new(name, null, null, true);
        TestValidationResult<CreateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_fail_when_district_exceeds_max_length()
    {
        string district = new('A', 101);
        CreateZoneCommand command = new("Zone A", district, null, true);
        TestValidationResult<CreateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.District);
    }

    [Fact]
    public void Should_fail_when_city_exceeds_max_length()
    {
        string city = new('A', 101);
        CreateZoneCommand command = new("Zone A", null, city, true);
        TestValidationResult<CreateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }
}
