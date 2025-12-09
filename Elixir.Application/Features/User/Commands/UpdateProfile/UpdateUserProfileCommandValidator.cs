using FluentValidation;

namespace Elixir.Application.Features.User.Commands.UpdateProfile;

public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
       
        RuleFor(x => x.UpdateProfileDto).NotNull().WithMessage("Profile data is required.");

        When(x => x.UpdateProfileDto != null, () =>
        {
            RuleFor(x => x.UpdateProfileDto.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(50).WithMessage("First name must not exceed 50 characters.");

            RuleFor(x => x.UpdateProfileDto.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(50).WithMessage("Last name must not exceed 50 characters.");

            RuleFor(x => x.UpdateProfileDto.EmailId)
                .NotEmpty().WithMessage("Profile email is required.")
                .EmailAddress().WithMessage("Profile email must be a valid email address.");

            RuleFor(x => x.UpdateProfileDto.PhoneNo)
                .NotEmpty().WithMessage("Phone number is required.")
                .MaximumLength(20).WithMessage("Phone number must not exceed 10 characters.");
        });
    }
}
