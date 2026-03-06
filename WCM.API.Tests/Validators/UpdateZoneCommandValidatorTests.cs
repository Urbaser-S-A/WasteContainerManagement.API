using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Zones.UpdateZone;

namespace WCM.API.Tests.Validators;

public class UpdateZoneCommandValidatorTests
{
    private readonly UpdateZoneCommandValidator _validator = new();

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        UpdateZoneCommand command = new(Guid.NewGuid(), "Zone A", "Centro", "Madrid", true);
        TestValidationResult<UpdateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        UpdateZoneCommand command = new(Guid.Empty, "Zone A", null, null, true);
        TestValidationResult<UpdateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_fail_when_name_is_empty()
    {
        UpdateZoneCommand command = new(Guid.NewGuid(), "", null, null, true);
        TestValidationResult<UpdateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public void Should_fail_when_name_exceeds_max_length()
    {
        string name = new('A', 101);
        UpdateZoneCommand command = new(Guid.NewGuid(), name, null, null, true);
        TestValidationResult<UpdateZoneCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }
}
