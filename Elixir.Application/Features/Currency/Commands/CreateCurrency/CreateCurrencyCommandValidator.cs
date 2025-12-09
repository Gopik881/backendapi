using FluentValidation;

namespace Elixir.Application.Features.Currency.Commands.CreateCurrency;

public class CreateCurrencyCommandValidator : AbstractValidator<CreateCurrencyCommand>
{
    public CreateCurrencyCommandValidator()
    {
        RuleFor(x => x.CreateCurrencyDto)
            .NotNull()
            .NotEmpty().WithMessage("At least one currency must be provided.");

        RuleForEach(x => x.CreateCurrencyDto).ChildRules(currency =>
        {
            currency.RuleFor(c => c.CurrencyName)
                .NotEmpty().WithMessage("Currency name is required.")
                .MaximumLength(50).WithMessage("Currency name must not exceed 50 characters.");
            currency.RuleFor(c => c.CurrencyShortName)
                .NotEmpty().WithMessage("Currency ShortName is required.")
                .MaximumLength(50).WithMessage("Currency ShortName must not exceed 50 characters.");
        });
    }
}

