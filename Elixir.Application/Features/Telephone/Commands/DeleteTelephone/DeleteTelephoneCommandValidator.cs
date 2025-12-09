using FluentValidation;

namespace Elixir.Application.Features.Telephone.Commands.DeleteTelephone;
public class DeleteTelephoneCommandValidator : AbstractValidator<DeleteTelephoneCommand>
{
    public DeleteTelephoneCommandValidator()
    {
        RuleFor(x => x.TelephoneCodeId)
            .GreaterThan(0).WithMessage("Telephone Code ID must be greater than zero.");
    }
}
