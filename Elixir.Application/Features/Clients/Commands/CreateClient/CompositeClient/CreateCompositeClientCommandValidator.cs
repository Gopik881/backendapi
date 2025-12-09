using Elixir.Application.Features.Clients.Commands.CreateClient.CompositeClient;
using Elixir.Application.Features.Clients.DTOs;
using FluentValidation;

namespace Elixir.Application.Features.Clients.Commands.CreateClient.CompositeClientCommandHandler;

public class CreateCompositeClientCommandValidator : AbstractValidator<CompositeClientCommand>
{
    public CreateCompositeClientCommandValidator()
    {
        #region clients validation
        // ClientInfo length and required validations
        RuleFor(x => x.CreateClientDto.ClientInfo.ClientName)
            .NotEmpty().WithMessage("Client name is required.")
            .MaximumLength(50).WithMessage("Client name cannot exceed 50 characters.");

        RuleFor(x => x.CreateClientDto.ClientInfo.ClientInfo)
            .MaximumLength(50).WithMessage("Client info cannot exceed 50 characters.");

        RuleFor(x => x.CreateClientDto.ClientInfo.ClientCode)
            .NotEmpty().WithMessage("Client code is required.")
            .MaximumLength(12).WithMessage("Client code cannot exceed 12 characters.");

        RuleFor(x => x.CreateClientDto.ClientInfo.Address1)
            .MaximumLength(255).WithMessage("Address1 cannot exceed 255 characters.");

        RuleFor(x => x.CreateClientDto.ClientInfo.Address2)
            .MaximumLength(255).WithMessage("Address2 cannot exceed 255 characters.");

        RuleFor(x => x.CreateClientDto.ClientInfo.ZipCode)
            .MaximumLength(20).WithMessage("ZipCode cannot exceed 20 characters.");

        RuleFor(x => x.CreateClientDto.ClientInfo.PhoneNumber)
            .MaximumLength(20).WithMessage("PhoneNumber cannot exceed 20 characters.");
        #endregion

        #region client contact info validation
        // Validate each ClientContactInfo item in the list
        RuleForEach(x => x.CreateClientDto.ClientContactInfo).ChildRules(contact =>
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
        When(x => x.CreateClientDto.ClientAdminInfo != null, () =>
        {
            RuleFor(x => x.CreateClientDto.ClientAdminInfo.FirstName)
                .NotEmpty().WithMessage("Admin first name is required.")
                .MaximumLength(45).WithMessage("Admin first name cannot exceed 45 characters.");

            RuleFor(x => x.CreateClientDto.ClientAdminInfo.LastName)
                .NotEmpty().WithMessage("Admin last name is required.")
                .MaximumLength(45).WithMessage("Admin last name cannot exceed 45 characters.");

            RuleFor(x => x.CreateClientDto.ClientAdminInfo.PhoneNumber)
                .MaximumLength(20).WithMessage("Admin phone number cannot exceed 20 characters.");

            RuleFor(x => x.CreateClientDto.ClientAdminInfo.Email)
                .NotEmpty().WithMessage("Admin email is required.")
                .MaximumLength(45).WithMessage("Admin email cannot exceed 45 characters.")
                .EmailAddress().WithMessage("Invalid admin email format.");

            RuleFor(x => x.CreateClientDto.ClientAdminInfo.Designation)
                .MaximumLength(50).WithMessage("Admin designation cannot exceed 50 characters.");
        });
        #endregion

    }
}
