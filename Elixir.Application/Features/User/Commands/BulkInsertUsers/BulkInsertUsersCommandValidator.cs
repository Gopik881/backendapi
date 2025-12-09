using FluentValidation;

namespace Elixir.Application.Features.User.Commands.BulkInsertUsers;

public class BulkInsertUsersCommandValidator:AbstractValidator<BulkInsertUsersCommand>
{
    public BulkInsertUsersCommandValidator()
    {
        RuleFor(x => x.Users).NotEmpty().WithMessage("User list cannot be empty.");
        RuleForEach(x => x.Users).ChildRules(user =>
        {
            user.RuleFor(u => u.FirstName).NotEmpty().WithMessage("First name is required.");
            user.RuleFor(u => u.LastName).NotEmpty().WithMessage("Last name is required.");
            user.RuleFor(u => u.Email).NotEmpty().EmailAddress().WithMessage("Valid email is required.");
            user.RuleFor(u => u.TelephonePhoneNumber).NotEmpty().WithMessage("Phone number is required.");
            user.RuleFor(u => u.TelephoneCode).NotEmpty().WithMessage("Phone code is required.");
            user.RuleFor(u => u.Designation).NotEmpty().WithMessage("Designation is required.");
            user.RuleFor(u => u.Location).NotEmpty().WithMessage("Location is required.");
        });
    }
}
