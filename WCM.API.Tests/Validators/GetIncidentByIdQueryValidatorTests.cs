using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Incidents.GetIncidentById;

namespace WCM.API.Tests.Validators;

public class GetIncidentByIdQueryValidatorTests
{
    private readonly GetIncidentByIdQueryValidator _validator = new();

    [Fact]
    public void Should_pass_when_id_is_valid()
    {
        GetIncidentByIdQuery query = new(Guid.NewGuid());
        TestValidationResult<GetIncidentByIdQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_id_is_empty()
    {
        GetIncidentByIdQuery query = new(Guid.Empty);
        TestValidationResult<GetIncidentByIdQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}
