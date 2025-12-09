using FluentValidation;


namespace Elixir.Application.Features.Currency.Commands.BulkInsertCurrencies;

public class BulkInsertCurrencyCommandValidator : AbstractValidator<BulkInsertCurrencyCommand>
{
    public BulkInsertCurrencyCommandValidator()
    {
        RuleFor(x => x.Currencies)
            .NotEmpty().WithMessage("Currency list cannot be empty.");
        RuleForEach(x => x.Currencies).ChildRules(currency =>
        {
            currency.RuleFor(c => c.CountryName)
                .NotEmpty().WithMessage("Country name is required.")
                .MaximumLength(50).WithMessage("Country name cannot exceed 50 characters.");
            currency.RuleFor(c => c.CurrencyName)
                .NotEmpty().WithMessage("Currency name is required.")
                .MaximumLength(255).WithMessage("Currency name cannot exceed 255 characters.");
            currency.RuleFor(c => c.CurrencyShortName)
                .NotEmpty().WithMessage("Currency short name is required.")
                .MaximumLength(10).WithMessage("Currency short name cannot exceed 10 characters.");
            currency.RuleFor(c => c.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        });
    }
}

