using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Zones.GetZones;

namespace WCM.API.Tests.Validators;

public class GetZonesQueryValidatorTests
{
    private readonly GetZonesQueryValidator _validator = new();

    [Fact]
    public void Should_pass_when_all_filters_are_null()
    {
        GetZonesQuery query = new(null, null, null);
        TestValidationResult<GetZonesQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_pass_when_filters_are_valid()
    {
        GetZonesQuery query = new("Centro", "Madrid", true);
        TestValidationResult<GetZonesQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_fail_when_district_exceeds_max_length()
    {
        string district = new('A', 101);
        GetZonesQuery query = new(district, null, null);
        TestValidationResult<GetZonesQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.District);
    }

    [Fact]
    public void Should_fail_when_city_exceeds_max_length()
    {
        string city = new('A', 101);
        GetZonesQuery query = new(null, city, null);
        TestValidationResult<GetZonesQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.City);
    }
}
