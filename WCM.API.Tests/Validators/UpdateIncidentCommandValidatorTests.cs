using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Incidents.UpdateIncident;
using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.Tests.Validators;

public class UpdateIncidentCommandValidatorTests
{
    private readonly UpdateIncidentCommandValidator _validator = new();

    private static UpdateIncidentCommand ValidCommand() => new(
        Guid.NewGuid(), IncidentType.Damage, "Container damaged",
        IncidentStatus.InProgress, IncidentPriority.High, null);

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        TestValidationResult<UpdateIncidentCommand> result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        UpdateIncidentCommand command = ValidCommand() with { Id = Guid.Empty };
        TestValidationResult<UpdateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_fail_when_type_is_invalid_enum()
    {
        UpdateIncidentCommand command = ValidCommand() with { Type = (IncidentType)999 };
        TestValidationResult<UpdateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Should_fail_when_status_is_invalid_enum()
    {
        UpdateIncidentCommand command = ValidCommand() with { Status = (IncidentStatus)999 };
        TestValidationResult<UpdateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }

    [Fact]
    public void Should_fail_when_priority_is_invalid_enum()
    {
        UpdateIncidentCommand command = ValidCommand() with { Priority = (IncidentPriority)999 };
        TestValidationResult<UpdateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }

    [Fact]
    public void Should_fail_when_description_exceeds_max_length()
    {
        UpdateIncidentCommand command = ValidCommand() with { Description = new('A', 1001) };
        TestValidationResult<UpdateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}
