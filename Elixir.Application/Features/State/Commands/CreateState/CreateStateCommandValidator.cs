using FluentValidation;
namespace Elixir.Application.Features.State.Commands.CreateState;

public class CreateStateCommandValidator : AbstractValidator<CreateStateCommand>
{
    public CreateStateCommandValidator()
    {
        RuleFor(x => x.CreateStateDto)
            .NotEmpty().WithMessage("At least one state is required.");

        RuleForEach(x => x.CreateStateDto).ChildRules(state =>
        {
            state.RuleFor(s => s.StateName)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(50).WithMessage("State name must not exceed 50 characters.");
            state.RuleFor(s => s.StateShortName)
                .NotEmpty().WithMessage("State short Name is required.")
                .MaximumLength(50).WithMessage("State short name must not exceed 50 characters.");
            state.RuleFor(s => s.CountryId)
                .NotEmpty().WithMessage("Country Id is required.");
        });
    }
}
