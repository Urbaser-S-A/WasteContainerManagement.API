using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Containers.UpdateContainer;
using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.Tests.Validators;

public class UpdateContainerCommandValidatorTests
{
    private readonly UpdateContainerCommandValidator _validator = new();

    private static UpdateContainerCommand ValidCommand() => new(
        Guid.NewGuid(), "CNT-001", Guid.NewGuid(), Guid.NewGuid(),
        40.4168, -3.7038, "Calle Mayor 1",
        1100, ContainerStatus.Active, DateTime.UtcNow, null);

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        TestValidationResult<UpdateContainerCommand> result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        UpdateContainerCommand command = ValidCommand() with { Id = Guid.Empty };
        TestValidationResult<UpdateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact]
    public void Should_fail_when_code_is_empty()
    {
        UpdateContainerCommand command = ValidCommand() with { Code = "" };
        TestValidationResult<UpdateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Should_fail_when_capacity_liters_is_zero()
    {
        UpdateContainerCommand command = ValidCommand() with { CapacityLiters = 0 };
        TestValidationResult<UpdateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CapacityLiters);
    }

    [Fact]
    public void Should_fail_when_status_is_invalid_enum()
    {
        UpdateContainerCommand command = ValidCommand() with { Status = (ContainerStatus)999 };
        TestValidationResult<UpdateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
