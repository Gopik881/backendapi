using Elixir.Application.Features.Country.Commands.BulkInsert;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Country.Commands.BulkInsertCountries;

public class BulkInsertCountriesCommandValidator : AbstractValidator<BulkInsertCountriesCommand>
{
    public BulkInsertCountriesCommandValidator()
    {
        RuleFor(x => x.Countries)
            .NotEmpty().WithMessage("Countries list cannot be empty.");
        RuleForEach(x => x.Countries).ChildRules(country =>
        {
            country.RuleFor(c => c.CountryName)
                .NotEmpty().WithMessage("Country name is required.")
                .MaximumLength(50).WithMessage("Country name cannot exceed 50 characters.");
            country.RuleFor(c => c.CountryShortName)
                .NotEmpty().WithMessage("Country short name is required.")
                .MaximumLength(10).WithMessage("Country short name cannot exceed 10 characters.");
            country.RuleFor(c => c.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        });
    }
}
