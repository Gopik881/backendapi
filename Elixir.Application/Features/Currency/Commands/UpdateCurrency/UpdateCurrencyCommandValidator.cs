using FluentValidation;

namespace Elixir.Application.Features.Currency.Commands.UpdateCurrency;

public class UpdateCurrencyCommandValidator : AbstractValidator<UpdateCurrencyCommand>
{
    public UpdateCurrencyCommandValidator()
    {
        RuleFor(x => x.UpdateCurrencyDto.CurrencyName)
            .NotEmpty().WithMessage("Currency Name is required.")
            .MaximumLength(50).WithMessage("Currency Name must not exceed 50 characters.");
        RuleFor(x => x.UpdateCurrencyDto.CurrencyShortName)
            .NotEmpty().WithMessage("Currency Short Name is required.")
            .MaximumLength(50).WithMessage("Currency Short Name must not exceed 50 characters.");
    }
}

