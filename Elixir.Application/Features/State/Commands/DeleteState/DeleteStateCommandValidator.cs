using FluentValidation;

namespace Elixir.Application.Features.State.Commands.DeleteState;
public class DeleteStateCommandValidator : AbstractValidator<DeleteStateCommand>
{
    public DeleteStateCommandValidator()
    {
        RuleFor(x => x.StateId)
            .GreaterThan(0).WithMessage("State ID must be greater than zero."); 
    }

}
