using FluentValidation;

namespace WCM.API.ApiService.Application.WasteTypes.DeleteWasteType;

public class DeleteWasteTypeCommandValidator : AbstractValidator<DeleteWasteTypeCommand>
{
    public DeleteWasteTypeCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .NotEqual(Guid.Empty);
    }
}
