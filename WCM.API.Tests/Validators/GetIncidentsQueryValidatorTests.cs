using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Incidents.GetIncidents;

namespace WCM.API.Tests.Validators;

public class GetIncidentsQueryValidatorTests
{
    private readonly GetIncidentsQueryValidator _validator = new();

    [Fact]
    public void Should_pass_when_pagination_is_valid()
    {
        GetIncidentsQuery query = new(null, null, null, null, null, null, 1, 20);
        TestValidationResult<GetIncidentsQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_fail_when_page_is_less_than_one(int page)
    {
        GetIncidentsQuery query = new(null, null, null, null, null, null, page, 20);
        TestValidationResult<GetIncidentsQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Should_fail_when_page_size_is_out_of_range(int pageSize)
    {
        GetIncidentsQuery query = new(null, null, null, null, null, null, 1, pageSize);
        TestValidationResult<GetIncidentsQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }
}
