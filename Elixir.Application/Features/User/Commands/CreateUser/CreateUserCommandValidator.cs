using FluentValidation;

namespace Elixir.Application.Features.User.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.UserProfileDto)
            .NotNull().WithMessage("User profile data is required.");

        When(x => x.UserProfileDto != null, () =>
        {
            RuleFor(x => x.UserProfileDto.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.UserProfileDto.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.UserProfileDto.EmailId)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");
            
            RuleFor(x => x.UserProfileDto.EmployeeLocation)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(50).WithMessage("Location must not exceed 50 characters.");

            RuleFor(x => x.UserProfileDto.Designation)
               .NotEmpty().WithMessage("Designation is required.")
               .MaximumLength(50).WithMessage("Designation must not exceed 50 characters.");
            // Add more rules as needed for other properties
        });
    }
}
