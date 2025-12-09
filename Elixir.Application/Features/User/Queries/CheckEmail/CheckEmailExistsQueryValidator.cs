using FluentValidation;
namespace Elixir.Application.Features.User.Queries.CheckEmail;

public class CheckEmailExistsQueryValidator : AbstractValidator<CheckEmailExistsQuery>
{
    public CheckEmailExistsQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }
}
