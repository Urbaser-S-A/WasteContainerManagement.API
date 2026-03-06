using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Incidents.CreateIncident;
using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.Tests.Validators;

public class CreateIncidentCommandValidatorTests
{
    private readonly CreateIncidentCommandValidator _validator = new();

    private static CreateIncidentCommand ValidCommand() => new(
        Guid.NewGuid(), IncidentType.Overflow, "Container overflowing", IncidentPriority.High);

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        TestValidationResult<CreateIncidentCommand> result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_pass_when_description_is_null()
    {
        CreateIncidentCommand command = ValidCommand() with { Description = null };
        TestValidationResult<CreateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_container_id_is_empty()
    {
        CreateIncidentCommand command = ValidCommand() with { ContainerId = Guid.Empty };
        TestValidationResult<CreateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ContainerId);
    }

    [Fact]
    public void Should_fail_when_type_is_invalid_enum()
    {
        CreateIncidentCommand command = ValidCommand() with { Type = (IncidentType)999 };
        TestValidationResult<CreateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Type);
    }

    [Fact]
    public void Should_fail_when_description_exceeds_max_length()
    {
        CreateIncidentCommand command = ValidCommand() with { Description = new('A', 1001) };
        TestValidationResult<CreateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_fail_when_priority_is_invalid_enum()
    {
        CreateIncidentCommand command = ValidCommand() with { Priority = (IncidentPriority)999 };
        TestValidationResult<CreateIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Priority);
    }
}
