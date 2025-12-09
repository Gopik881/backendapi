using FluentValidation;

namespace Elixir.Application.Features.User.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        When(x => x.UpdateUserDto != null, () =>
        {
            RuleFor(x => x.UpdateUserDto.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.UpdateUserDto.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.UpdateUserDto.EmailId)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            RuleFor(x => x.UpdateUserDto.EmployeeLocation)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(50).WithMessage("Location must not exceed 50 characters.");

            RuleFor(x => x.UpdateUserDto.Designation)
               .NotEmpty().WithMessage("Designation is required.")
               .MaximumLength(50).WithMessage("Designation must not exceed 50 characters.");
            // Add more rules as needed for other properties
        });
    }
}
