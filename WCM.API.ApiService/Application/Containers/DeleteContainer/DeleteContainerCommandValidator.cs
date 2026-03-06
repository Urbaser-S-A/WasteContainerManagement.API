using FluentValidation;

namespace WCM.API.ApiService.Application.Containers.DeleteContainer;

public class DeleteContainerCommandValidator : AbstractValidator<DeleteContainerCommand>
{
    public DeleteContainerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
