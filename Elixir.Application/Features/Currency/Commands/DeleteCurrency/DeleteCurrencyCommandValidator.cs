using FluentValidation;

namespace Elixir.Application.Features.Currency.Commands.DeleteCurrency;

public class DeleteCurrencyCommandValidator : AbstractValidator<DeleteCurrencyCommand>
{
    public DeleteCurrencyCommandValidator()
    {
        RuleFor(x => x.CurrencyId)
            .GreaterThan(0).WithMessage("Currency ID must be greater than zero.");
    }
}

