using FluentValidation;

namespace Elixir.Application.Features.Clients.Commands.UpdateClient.UpdateClient.UpdateClientCompositeCommand;

public class UpdateClientCompositeCommandValidator : AbstractValidator<UpdateClientCompositeCommand>
{
    public UpdateClientCompositeCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("UserId must be greater than 0.");

        RuleFor(x => x.ClientId)
            .GreaterThan(0).WithMessage("ClientId must be greater than 0.");

        #region clients validation
        // ClientInfo length and required validations
        RuleFor(x => x.UpdateClientDto.ClientInfo.ClientName)
            .NotEmpty().WithMessage("Client name is required.")
            .MaximumLength(50).WithMessage("Client name cannot exceed 50 characters.");

        RuleFor(x => x.UpdateClientDto.ClientInfo.ClientInfo)
            .MaximumLength(50).WithMessage("Client info cannot exceed 50 characters.");

        RuleFor(x => x.UpdateClientDto.ClientInfo.ClientCode)
            .NotEmpty().WithMessage("Client code is required.")
            .MaximumLength(12).WithMessage("Client code cannot exceed 12 characters.");

        RuleFor(x => x.UpdateClientDto.ClientInfo.Address1)
            .MaximumLength(255).WithMessage("Address1 cannot exceed 255 characters.");

        RuleFor(x => x.UpdateClientDto.ClientInfo.Address2)
            .MaximumLength(255).WithMessage("Address2 cannot exceed 255 characters.");

        RuleFor(x => x.UpdateClientDto.ClientInfo.ZipCode)
            .MaximumLength(20).WithMessage("ZipCode cannot exceed 20 characters.");

        RuleFor(x => x.UpdateClientDto.ClientInfo.PhoneNumber)
            .MaximumLength(20).WithMessage("PhoneNumber cannot exceed 20 characters.");
        #endregion

        #region client contact info validation
        // Validate each ClientContactInfo item in the list
        RuleForEach(x => x.UpdateClientDto.ClientContactInfo).ChildRules(contact =>
        {
            contact.RuleFor(c => c.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(45).WithMessage("First name cannot exceed 45 characters.");

            contact.RuleFor(c => c.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(45).WithMessage("Last name cannot exceed 45 characters.");

            contact.RuleFor(c => c.PhoneNumber)
                .MaximumLength(20).WithMessage("Phone number cannot exceed 20 characters.");

            contact.RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required.")
                .MaximumLength(255).WithMessage("Email cannot exceed 255 characters.")
                .EmailAddress().WithMessage("Invalid email format.");

            contact.RuleFor(c => c.Designation)
                .MaximumLength(50).WithMessage("Designation cannot exceed 50 characters.");

            contact.RuleFor(c => c.Department)
                .MaximumLength(45).WithMessage("Department cannot exceed 45 characters.");

            contact.RuleFor(c => c.Remarks)
                .MaximumLength(2000).WithMessage("Remarks cannot exceed 2000 characters.");
        });

        #endregion

        #region client admin info validation
        // Validate ClientAdminInfo fields
        RuleFor(x => x.UpdateClientDto.ClientAdminInfo.FirstName)
            .NotEmpty().WithMessage("Admin first name is required.")
            .MaximumLength(45).WithMessage("Admin first name cannot exceed 45 characters.");

        RuleFor(x => x.UpdateClientDto.ClientAdminInfo.LastName)
            .NotEmpty().WithMessage("Admin last name is required.")
            .MaximumLength(45).WithMessage("Admin last name cannot exceed 45 characters.");

        RuleFor(x => x.UpdateClientDto.ClientAdminInfo.PhoneNumber)
            .MaximumLength(20).WithMessage("Admin phone number cannot exceed 20 characters.");

        RuleFor(x => x.UpdateClientDto.ClientAdminInfo.Email)
            .NotEmpty().WithMessage("Admin email is required.")
            .MaximumLength(45).WithMessage("Admin email cannot exceed 45 characters.")
            .EmailAddress().WithMessage("Invalid admin email format.");

        RuleFor(x => x.UpdateClientDto.ClientAdminInfo.Designation)
            .MaximumLength(50).WithMessage("Admin designation cannot exceed 50 characters.");
        #endregion

    }
}
