using FluentValidation;

namespace WCM.API.ApiService.Application.Containers.GetContainers;

public class GetContainersQueryValidator : AbstractValidator<GetContainersQuery>
{
    public GetContainersQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);
    }
}
