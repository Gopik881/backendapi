using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ApproveCompanyOnBoardingStatus;
using Elixir.Application.Features.Company.Commands.CreateCompany.CompanyData;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using FluentValidation;
using MediatR;
using System.ComponentModel.Design;

namespace Elixir.Application.Features.Company.Commands.CreateCompany.CompositeCompany5Tab;

public record CompositeCompany5TabCommand(int UserId, int CompanyId, Company5TabDto Company5TabDto) : IRequest<bool>;

public class CompositeCompany5TabCommandHandler : IRequestHandler<CompositeCompany5TabCommand, bool>
{
    private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
    private readonly ICompaniesRepository _compniesRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly ICryptoService _cryptoService;
    private readonly IAccountHistoryRepository _accountHistoryRepository;
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    private readonly IMediator _mediator;
    private readonly INotificationsRepository _notificationsRepository;
    public CompositeCompany5TabCommandHandler(Func<ITransactionRepository> transactionRepositoryFactory,
        ICompaniesRepository companiesRepository, IUsersRepository usersRepository,
        ICryptoService cryptoService, IAccountHistoryRepository accountHistoryRepository,
        ICompanyOnboardingStatusRepository companyOnboardingStatusRepository, IMediator mediator, INotificationsRepository notificationsRepository)
    {
        _transactionRepositoryFactory = transactionRepositoryFactory;
        _compniesRepository = companiesRepository;
        _usersRepository = usersRepository;
        _cryptoService = cryptoService;
        _accountHistoryRepository = accountHistoryRepository;
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
        _mediator = mediator;
        _notificationsRepository = notificationsRepository;

    }
    public async Task<bool> Handle(CompositeCompany5TabCommand request, CancellationToken cancellationToken)
    {
        //var companyDto = request.Company5TabDto.Company5TabCompanyDto;
        //var accountDto = request.Company5TabDto.Company5TabAccountDto;
        //var adminDto = request.Company5TabDto.Company5TabCompanyAdminDto;

        //// Check if company data already exists
        //var errorMessages = new List<string>();

        //if (await _companyOnboardingStatusRepository.IsCompanyOnboardingStatusDataExistsAsync(companyDto.CompanyId))
        //    errorMessages.Add("Company onboarding status data already exists.");

        //// Check if company code exists
        //if (!string.IsNullOrEmpty(companyDto.CompanyCode) && await _compniesRepository.ExistsWithCompanyCodeAsync(companyDto.CompanyCode))
        //    errorMessages.Add("Company code already exists.");

        //// Check if email exists
        //if (!string.IsNullOrEmpty(adminDto.CompanyAdminEmailId) && await _usersRepository.ExistsUserByEmailAsync(adminDto.CompanyAdminEmailId, _cryptoService.GenerateIntegerHashForString(adminDto.CompanyAdminEmailId)))
        //    errorMessages.Add("Company admin email already exists.");

        //// Check if PAN, TAN, GSTIN, ContractId exist
        //if (!string.IsNullOrEmpty(accountDto.Pan) && await _accountHistoryRepository.IsPanExistsAsync(accountDto.Pan, companyDto.CompanyId))
        //    errorMessages.Add("PAN already exists.");

        //if (!string.IsNullOrEmpty(accountDto.Tan) && await _accountHistoryRepository.IsTanExistsAsync(accountDto.Tan, companyDto.CompanyId))
        //    errorMessages.Add("TAN already exists.");

        //if (!string.IsNullOrEmpty(accountDto.Gstn) && await _accountHistoryRepository.IsGstInExistsAsync(accountDto.Gstn, companyDto.CompanyId))
        //    errorMessages.Add("GSTIN already exists.");

        //if (!string.IsNullOrEmpty(accountDto.ContractId) && await _accountHistoryRepository.IsContractIdExistsAsync(accountDto.ContractId, companyDto.CompanyId))
        //    errorMessages.Add("ContractId already exists.");

        //if (errorMessages.Any())
        //    return errorMessages;

        using var transactionRepository = _transactionRepositoryFactory();
        await transactionRepository.BeginTransactionAsync();
        try
        {
            // Save company data
            var companyStoreageGB = request.Company5TabDto.Company5TabAccountDto.CompanyStorageGB ?? 0;
            var perUserStorageGB = request.Company5TabDto.Company5TabAccountDto.PerUserStorageMB ?? 0;
            var companyCommand = new CreateCompany5TabCommand(request.UserId, request.Company5TabDto.Company5TabCompanyDto, companyStoreageGB, perUserStorageGB);
            if (!await _mediator.Send(companyCommand))
                throw new Exception("Failed to create Company data.");
            //Save Account data
            var accountCommand = new CreateCompany5TabAccountCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabAccountDto);
            if (!await _mediator.Send(accountCommand))
                throw new Exception("Failed to create Account data.");
            var adminCommand = new CreateCompany5TabCompanyAdminCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabCompanyAdminDto);
            if (!await _mediator.Send(adminCommand))
                throw new Exception("Failed to create Company Admin data.");
            var moduleMappingCommand = new CreateCompany5TabModuleMappingCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabModuleMappingDto);
            if (!await _mediator.Send(moduleMappingCommand))
                throw new Exception("Failed to create Module Mapping data.");
            var reportingToolLimitsCommand = new CreateCompany5TabReportingToolLimitsCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabReportingToolLimitsDto);
            if (!await _mediator.Send(reportingToolLimitsCommand))
                throw new Exception("Failed to create Reporting Tool Limits data.");
            //if (request.Company5TabDto.Company5TabEscalationContactDto.Count > 0)
            //{
                var escalationContactCommand = new CreateCompany5TabEscalationContactCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabEscalationContactDto);
                if (!await _mediator.Send(escalationContactCommand))
                    throw new Exception("Failed to create Escalation Contacts data.");
            //}
            //if (request.Company5TabDto.company5TabElixirUserDto.Count > 0)
            //{
                var elixirUsersCommand = new CreateCompany5TabElixirUsersCommand(request.CompanyId, request.UserId, request.Company5TabDto.company5TabElixirUserDto);
                if (!await _mediator.Send(elixirUsersCommand))
                    throw new Exception("Failed to create Elixir Users data.");
            //}

            var OnboardingHistoryCommand = new CreateCompany5TabOnboardingHistoryCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(OnboardingHistoryCommand))
                throw new Exception("Failed to update onBoarding History data.");
            await transactionRepository.CommitAsync();


            // Pseudocode:
            // 1. After all company 5-tab creation commands succeed, get onboarding status.
            // 2. Create NotificationDto with relevant details: 
            //    - Title: "Company5Tab"
            //    - Message: "Company 5 Tab data created successfully. Current onboarding status: {onboardingStatus}"
            //    - Set CompanyId, UserId, CreatedAt, UpdatedAt, IsRead, IsDeleted, IsActive, NotificationType.
            // 3. Call _notificationsRepository.InsertNotificationAsync(notification).

            // Place this after successful transaction commit, before returning true:

            // Retrieve the company entity by its ID
            var companyEntity = await _compniesRepository.GetCompanyByIdAsync(request.CompanyId);

            // Get the current onboarding status for the company
            var currentOnboardingStatus = await _companyOnboardingStatusRepository.GetCompanyOnBoardingStatus(request.CompanyId);

            // Fetch all user IDs for account managers and checkers associated with this company
            var accountManagerAndCheckerUserIds = await _usersRepository.GetAccountManagersAndCheckersUserIdsAsync(request.CompanyId);

            // Send a notification to each account manager and checker
            foreach (var userId in accountManagerAndCheckerUserIds)
            {
                var notification = new NotificationDto
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    UserId = userId,
                    CompanyId = request.CompanyId,
                    Title = "Company5Tab",
                    Message = $"Company 5 Tab data created successfully for company '{companyEntity?.CompanyName}'. Current onboarding status: {currentOnboardingStatus}",
                    NotificationType = "Info",
                    IsRead = false,
                    IsDeleted = false,
                    // Notification is active only if onboarding is approved
                    IsActive = currentOnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED
                };
                await _notificationsRepository.InsertNotificationAsync(notification);
            }

            return true;
        }
        catch (Exception ex)
        {
            await transactionRepository.RollbackAsync();
            //return false;
            throw new Exception("Failed to Create 5tab process Error: " + ex.InnerException ?? ex.Message);
        }
    }
}
