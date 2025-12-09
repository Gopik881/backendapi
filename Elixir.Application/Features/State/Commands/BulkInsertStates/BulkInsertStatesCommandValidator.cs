using FluentValidation;


namespace Elixir.Application.Features.State.Commands.BulkInsertStates;

public class BulkInsertStatesCommandValidator : AbstractValidator<BulkInsertStatesCommand>
{
    public BulkInsertStatesCommandValidator()
    {
        RuleFor(x => x.States)
            .NotEmpty().WithMessage("States list cannot be empty.");
        RuleForEach(x => x.States).ChildRules(state =>
        {
            state.RuleFor(s => s.StateName)
                .NotEmpty().WithMessage("State name is required.")
                .MaximumLength(50).WithMessage("State name cannot exceed 50 characters.");
            state.RuleFor(s => s.StateShortName)
                .NotEmpty().WithMessage("State short name is required.")
                .MaximumLength(10).WithMessage("State short name cannot exceed 10 characters.");
            state.When(s => s.CountryName != null, () =>
            {
                state.RuleFor(s => s.CountryName)
                    .NotEmpty().WithMessage("Country name is required.")
                    .MaximumLength(50).WithMessage("Country name cannot exceed 50 characters.");
            });
            state.RuleFor(s => s.Description)
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        });
    }
}
