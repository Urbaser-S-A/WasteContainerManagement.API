using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Containers.CreateContainer;
using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.Tests.Validators;

public class CreateContainerCommandValidatorTests
{
    private readonly CreateContainerCommandValidator _validator = new();

    private static CreateContainerCommand ValidCommand() => new(
        "CNT-001", Guid.NewGuid(), Guid.NewGuid(),
        40.4168, -3.7038, "Calle Mayor 1",
        1100, ContainerStatus.Active, DateTime.UtcNow);

    [Fact]
    public void Should_pass_when_all_fields_are_valid()
    {
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(ValidCommand());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Should_fail_when_code_is_empty_or_null(string? code)
    {
        CreateContainerCommand command = ValidCommand() with { Code = code! };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Should_fail_when_code_exceeds_max_length()
    {
        CreateContainerCommand command = ValidCommand() with { Code = new('A', 51) };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    [Fact]
    public void Should_fail_when_waste_type_id_is_empty()
    {
        CreateContainerCommand command = ValidCommand() with { WasteTypeId = Guid.Empty };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.WasteTypeId);
    }

    [Fact]
    public void Should_fail_when_zone_id_is_empty()
    {
        CreateContainerCommand command = ValidCommand() with { ZoneId = Guid.Empty };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.ZoneId);
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public void Should_fail_when_latitude_is_out_of_range(double latitude)
    {
        CreateContainerCommand command = ValidCommand() with { Latitude = latitude };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public void Should_fail_when_longitude_is_out_of_range(double longitude)
    {
        CreateContainerCommand command = ValidCommand() with { Longitude = longitude };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Longitude);
    }

    [Theory]
    [InlineData(-90)]
    [InlineData(0)]
    [InlineData(90)]
    public void Should_pass_when_latitude_is_at_boundary(double latitude)
    {
        CreateContainerCommand command = ValidCommand() with { Latitude = latitude };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveValidationErrorFor(x => x.Latitude);
    }

    [Fact]
    public void Should_fail_when_address_exceeds_max_length()
    {
        CreateContainerCommand command = ValidCommand() with { Address = new('A', 501) };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Address);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_fail_when_capacity_liters_is_not_positive(int capacity)
    {
        CreateContainerCommand command = ValidCommand() with { CapacityLiters = capacity };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.CapacityLiters);
    }

    [Fact]
    public void Should_fail_when_status_is_invalid_enum()
    {
        CreateContainerCommand command = ValidCommand() with { Status = (ContainerStatus)999 };
        TestValidationResult<CreateContainerCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Status);
    }
}
