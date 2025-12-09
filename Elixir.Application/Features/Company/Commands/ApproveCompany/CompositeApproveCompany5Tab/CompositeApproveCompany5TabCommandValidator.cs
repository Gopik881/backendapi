//using Elixir.Application.Features.Company.DTOs;
//using FluentValidation;

//namespace Elixir.Application.Features.Company.Commands.ApproveCompany.CompositeApproveCompany5Tab;

//    public class CompositeApproveCompany5TabCommandValidator : AbstractValidator<CompositeApproveCompany5TabCommand>
//{
   
//    public CompositeApproveCompany5TabCommandValidator()
//    {
//        RuleFor(x => x.UserId)
//            .GreaterThan(0)
//            .WithMessage("User ID must be greater than 0.");

//        RuleFor(x => x.CompanyId)
//            .GreaterThan(0)
//            .WithMessage("Company ID must be greater than 0.");

//        RuleFor(x => x.Company5TabDto)
//            .NotNull()
//            .WithMessage("Company5TabDto cannot be null.");

//        #region Company5TabAccountDto Validation
//        When(x => x.Company5TabDto != null && x.Company5TabDto.Company5TabAccountDto != null, () =>
//        {
//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto.PerUserStorageMB)
//                .GreaterThan(0)
//                .WithMessage("PerUserStorageMB must be greater than 0.");

//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto.UserGroupLimit)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("UserGroupLimit must be 0 or greater.");

//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto.TempUserLimit)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("TempUserLimit must be 0 or greater.");

//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto.ContractName)
//                .NotEmpty()
//                .MaximumLength(50)
//                .WithMessage("ContractName cannot be empty and must not exceed 50 characters.");

//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto.ContractNoticePeriod)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("ContractNoticePeriod must be 0 or greater.");

//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto.Pan)
//                .NotEmpty()
//                .MaximumLength(10)
//                .WithMessage("PAN cannot be empty and PAN must not exceed 10 characters.");

//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto.Tan)
//                .NotEmpty()
//                .MaximumLength(10)
//                .WithMessage("TAN cannot be empty and TAN must not exceed 10 characters.");

//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto.Gstn)
//                .NotEmpty()
//                .MaximumLength(15)
//                .WithMessage("GSTIN cannot be empty and GSTIN must not exceed 15 characters.");
//        }).Otherwise(() =>
//        {
//            RuleFor(x => x.Company5TabDto.Company5TabAccountDto)
//                .NotNull()
//                .WithMessage("Company5TabAccountDto cannot be null.");
//        });
//        #endregion

//        #region Company5TabCompanyAdminDto Validation
//        When(x => x.Company5TabDto != null && x.Company5TabDto.Company5TabCompanyAdminDto != null, () =>
//        {
//            RuleFor(x => x.Company5TabDto.Company5TabCompanyAdminDto.CompanyAdminFirstName)
//                .NotEmpty()
//                .MaximumLength(50)
//                .WithMessage("First name must not be empty and must not exceed 50 characters.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyAdminDto.CompanyAdminLastName)
//                .NotEmpty()
//                .MaximumLength(50)
//                .WithMessage("Last name must not be empty and must not exceed 50 characters.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyAdminDto.CompanyAdminEmailId)
//                .NotEmpty()
//                .WithMessage("Email ID must not be empty.")
//                .EmailAddress()
//                .WithMessage("Invalid email format.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyAdminDto.CompanyAdminPhoneNo)
//                .NotEmpty()
//                .WithMessage("Company Admin Info phone number must not be empty.")
//                .MaximumLength(15)
//                .WithMessage("Company Admin Info phone number must not exceed 15 digits.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyAdminDto.CompanyAdminDesignation)
//                .NotEmpty()
//                .MaximumLength(50)
//                .WithMessage("Designation must not be empty and must not exceed 50 characters.");
//        }).Otherwise(() =>
//        {
//            RuleFor(x => x.Company5TabDto.Company5TabCompanyAdminDto)
//                .NotNull()
//                .WithMessage("Company5TabCompanyAdminDto cannot be null.");
//        });
//        #endregion

//        #region Company5TabCompanyDto Validation
//        When(x => x.Company5TabDto != null && x.Company5TabDto.Company5TabCompanyDto != null, () =>
//        {
//            RuleFor(x => x.Company5TabDto.Company5TabCompanyDto.CompanyName)
//                .NotEmpty()
//                .MaximumLength(50)
//                .WithMessage("Company name must not be empty and must not exceed 50 characters.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyDto.CompanyCode)
//                .NotEmpty()
//                .WithMessage("Company code must not be empty.")
//                .MaximumLength(50)
//                .WithMessage("Company code must not exceed 50 characters.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyDto.Address1)
//                .NotEmpty()
//                .MaximumLength(50)
//                .WithMessage("Address1 must not be empty and must not exceed 50 characters.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyDto.StateId)
//                .GreaterThan(0)
//                .WithMessage("State ID must be greater than 0.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyDto.CountryId)
//                .GreaterThan(0)
//                .WithMessage("Country ID must be greater than 0.");

//            RuleFor(x => x.Company5TabDto.Company5TabCompanyDto.PhoneNumber)
//                .NotEmpty()
//                .WithMessage("Company Info Phone number must not be empty.")
//                .MaximumLength(15)
//                .WithMessage("Company Info Phone number must not exceed 15 digits.");
//        }).Otherwise(() =>
//        {
//            RuleFor(x => x.Company5TabDto.Company5TabCompanyDto)
//                .NotNull()
//                .WithMessage("Company5TabCompanyDto cannot be null.");
//        });
//        #endregion

//        #region ElixirUsersDto Validation
//        When(x => x.Company5TabDto != null && x.Company5TabDto.company5TabElixirUserDto != null, () =>
//        {
//            RuleFor(x => x.Company5TabDto.company5TabElixirUserDto)
//                .Must(dto => dto != null && dto.All(user => user.GroupId > 0 && user.UserId > 0))
//                .WithMessage("Each Elixir user must have a valid GroupId and UserId.");
//        }).Otherwise(() =>
//        {
//            RuleFor(x => x.Company5TabDto.company5TabElixirUserDto)
//                .NotNull()
//                .WithMessage("ElixirUsersDto cannot be null.");
//        });
//        #endregion

//        #region Company5TabEscalationContactDto Validation
//        When(x => x.Company5TabDto != null && x.Company5TabDto.Company5TabEscalationContactDto != null, () =>
//        {
//            RuleForEach(x => x.Company5TabDto.Company5TabEscalationContactDto).ChildRules(contact =>
//            {
//                contact.RuleFor(c => c.FirstName)
//                    .NotEmpty()
//                    .MaximumLength(50)
//                    .WithMessage("First name is required and must not exceed 50 characters.");

//                contact.RuleFor(c => c.LastName)
//                    .NotEmpty()
//                    .MaximumLength(50)
//                    .WithMessage("Last name is required and must not exceed 50 characters.");

//                contact.RuleFor(c => c.EmailId)
//                    .NotEmpty()
//                    .WithMessage("Email ID is required.")
//                    .EmailAddress()
//                    .WithMessage("Invalid email format.");

//                contact.RuleFor(c => c.PhoneNumber)
//                    .MaximumLength(15)
//                    .WithMessage("Escalation Contacts Phone number must not exceed 15 digits.");

//                contact.RuleFor(c => c.Designation)
//                    .NotEmpty()
//                    .MaximumLength(50)
//                    .WithMessage("Designation is required and must not exceed 50 characters.");

//                contact.RuleFor(c => c.Department)
//                    .NotEmpty()
//                    .MaximumLength(50)
//                    .WithMessage("Department is required and must not exceed 50 characters.");
//            });
//        }).Otherwise(() =>
//        {
//            RuleFor(x => x.Company5TabDto.Company5TabEscalationContactDto)
//                .NotNull()
//                .WithMessage("Company5TabEscalationContactDto cannot be null.");
//        });
//        #endregion

//        #region Company5TabModuleMappingDto Validation
//        RuleFor(x => x.Company5TabDto.Company5TabModuleMappingDto)
//            .NotNull()
//            .WithMessage("Module mappings cannot be null.")
//            .Must(mappings => mappings != null && mappings.Any())
//            .WithMessage("Module mappings must contain at least one item.");
//        #endregion

//        #region Company5TabReportingToolLimitsDto Validation
//        When(x => x.Company5TabDto != null && x.Company5TabDto.Company5TabReportingToolLimitsDto != null, () =>
//        {
//            // Replace this line:
//            // var r = x => x.Company5TabDto.Company5TabReportingToolLimitsDto;

//            // With this explicit delegate type:
//            Func<CompositeApproveCompany5TabCommand, Company5TabReportingToolLimitsDto> r = x => x.Company5TabDto.Company5TabReportingToolLimitsDto;

//            RuleFor(x => r(x).NoOfReportingAdmins)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("Number of Reporting Admins must be non-negative.");

//            RuleFor(x => r(x).NoOfCustomReportCreators)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("Number of Custom Report Creators must be non-negative.");

//            RuleFor(x => r(x).NoOfSavedReportQueriesCompany)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("Number of Saved Report Queries in Library must be non-negative.");

//            RuleFor(x => r(x).NoOfDashboardsCompany)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("Number of Dashboards in Library must be non-negative.");

//            RuleFor(x => r(x).NoOfSavedReportQueriesUsers)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("Number of Saved Report Queries per User must be non-negative.");

//            RuleFor(x => r(x).NoOfDashboardsUsers)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("Number of Dashboards in Personal Library must be non-negative.");

//            RuleFor(x => r(x).NoOfLetterGenerationAdmin)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("Number of Letter Generation Admins must be non-negative.");

//            RuleFor(x => r(x).NoOfTemplatesSaved)
//                .GreaterThanOrEqualTo(0)
//                .WithMessage("Number of Templates Saved must be non-negative.");
//        }).Otherwise(() =>
//        {
//            RuleFor(x => x.Company5TabDto.Company5TabReportingToolLimitsDto)
//                .NotNull()
//                .WithMessage("Company5TabReportingToolLimitsDto cannot be null.");
//        });
//        #endregion
//    }
//}

