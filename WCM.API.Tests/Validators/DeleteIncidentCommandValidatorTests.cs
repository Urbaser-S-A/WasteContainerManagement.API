using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Incidents.DeleteIncident;

namespace WCM.API.Tests.Validators;

public class DeleteIncidentCommandValidatorTests
{
    private readonly DeleteIncidentCommandValidator _validator = new();

    [Fact]
    public void Should_pass_when_id_is_valid()
    {
        DeleteIncidentCommand command = new(Guid.NewGuid());
        TestValidationResult<DeleteIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        DeleteIncidentCommand command = new(Guid.Empty);
        TestValidationResult<DeleteIncidentCommand> result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
