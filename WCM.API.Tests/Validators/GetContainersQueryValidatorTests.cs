using FluentValidation.TestHelper;
using WCM.API.ApiService.Application.Containers.GetContainers;
using WCM.API.ApiService.Domain.Enums;

namespace WCM.API.Tests.Validators;

public class GetContainersQueryValidatorTests
{
    private readonly GetContainersQueryValidator _validator = new();

    [Fact]
    public void Should_pass_when_pagination_is_valid()
    {
        GetContainersQuery query = new(null, null, null, 1, 20);
        TestValidationResult<GetContainersQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_pass_with_all_filters()
    {
        GetContainersQuery query = new(Guid.NewGuid(), Guid.NewGuid(), ContainerStatus.Active, 1, 50);
        TestValidationResult<GetContainersQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_fail_when_page_is_less_than_one(int page)
    {
        GetContainersQuery query = new(null, null, null, page, 20);
        TestValidationResult<GetContainersQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Page);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    [InlineData(-1)]
    public void Should_fail_when_page_size_is_out_of_range(int pageSize)
    {
        GetContainersQuery query = new(null, null, null, 1, pageSize);
        TestValidationResult<GetContainersQuery> result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.PageSize);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Should_pass_when_page_size_is_at_boundary(int pageSize)
    {
        GetContainersQuery query = new(null, null, null, 1, pageSize);
        TestValidationResult<GetContainersQuery> result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
