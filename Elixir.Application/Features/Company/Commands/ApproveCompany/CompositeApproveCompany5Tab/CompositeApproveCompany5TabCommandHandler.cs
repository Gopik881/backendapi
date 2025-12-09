using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.Commands.ApproveCompany.Account;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ApproveCompanyOnBoardingStatus;
using Elixir.Application.Features.Company.Commands.ApproveCompany.CompanyAdmin;
using Elixir.Application.Features.Company.Commands.ApproveCompany.CompanyData;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ElixirUser;
using Elixir.Application.Features.Company.Commands.ApproveCompany.EscalationContacts;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ModuleMapping;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ReportingToolLimit;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using FluentValidation;
using Microsoft.Extensions.Options;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.CompositeApproveCompany5Tab;

public record CompositeApproveCompany5TabCommand(int UserId, int CompanyId, Company5TabDto Company5TabDto) : IRequest<bool>;
public class CompositeApproveCompany5TabCommandHandler : IRequestHandler<CompositeApproveCompany5TabCommand, bool>
{
    private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
    private readonly ICryptoService _cryptoService;
    private readonly IMediator _mediator;
    private readonly INotificationsRepository _notificationsRepository;
    private readonly ICompaniesRepository _companiesRepository;
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    private readonly IUsersRepository _usersRepository;
    private readonly DBConfigSettings _dbConfigSettings;

    public CompositeApproveCompany5TabCommandHandler(Func<ITransactionRepository> transactionRepositoryFactory,
        IMediator mediator,
        ICryptoService cryptoService,
        INotificationsRepository notificationsRepository, ICompaniesRepository companiesRepository,
        ICompanyOnboardingStatusRepository companyOnboardingStatusRepository, IUsersRepository usersRepository,
        IOptions<DBConfigSettings> dbConfigSettings)
    {
        _transactionRepositoryFactory = transactionRepositoryFactory;
        _mediator = mediator;
        _cryptoService = cryptoService;
        _notificationsRepository = notificationsRepository;
        _companiesRepository = companiesRepository;
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
        _usersRepository = usersRepository;
        _dbConfigSettings = dbConfigSettings.Value;
    }

    public async Task<bool> Handle(CompositeApproveCompany5TabCommand request, CancellationToken cancellationToken)
    {
        using var transactionRepository = _transactionRepositoryFactory();
        await transactionRepository.BeginTransactionAsync();
        try
        {
            var companyStorageGB = request.Company5TabDto.Company5TabAccountDto.CompanyStorageGB ?? 0;
            var perUserStorageMB = request.Company5TabDto.Company5TabAccountDto.PerUserStorageMB ?? 0;
            // Save company data
            var companyCommand = new ApproveCompany5TabCommand(request.UserId, request.Company5TabDto.Company5TabCompanyDto, companyStorageGB, perUserStorageMB);
            if (!await _mediator.Send(companyCommand))
                throw new Exception("Failed to Approve Company data.");
            //Save Account data
            var accountCommand = new ApproveCompany5TabAccountCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabAccountDto);
            if (!await _mediator.Send(accountCommand))
                throw new Exception("Failed to Approve Account data.");
            var adminCommand = new ApproveCompany5TabCompanyAdminCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabCompanyAdminDto);
            if (!await _mediator.Send(adminCommand))
                throw new Exception("Failed to Approve Company Admin data.");
            var moduleMappingCommand = new ApproveCompany5TabModuleMappingCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabModuleMappingDto);
            if (!await _mediator.Send(moduleMappingCommand))
                throw new Exception("Failed to Approve Module Mapping data.");
            var reportingToolLimitsCommand = new ApproveCompany5TabReportingToolLimitsCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabReportingToolLimitsDto);
            if (!await _mediator.Send(reportingToolLimitsCommand))
                throw new Exception("Failed to Approve Reporting Tool Limits data.");
            if (request.Company5TabDto.Company5TabEscalationContactDto.Count > 0)
            {
                var escalationContactCommand = new ApproveCompany5TabEscalationContactCommand(request.CompanyId, request.UserId, request.Company5TabDto.Company5TabEscalationContactDto);
                if (!await _mediator.Send(escalationContactCommand))
                    throw new Exception("Failed to Approve Escalation Contacts data.");
            }
            if (request.Company5TabDto.company5TabElixirUserDto.Count > 0)
            {
                var elixirUsersCommand = new ApproveCompany5TabElixirUsersCommand(request.CompanyId, request.UserId, request.Company5TabDto.company5TabElixirUserDto);
                if (!await _mediator.Send(elixirUsersCommand))
                    throw new Exception("Failed to Approve Elixir Users data.");
            }

            var OnboardingHistoryCommand = new ApproveCompany5TabOnboardingHistoryCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(OnboardingHistoryCommand))
                throw new Exception("Failed to update onBoarding History data.");

            await transactionRepository.CommitAsync();

            // Retrieve the company entity by its ID
            var companyEntity = await _companiesRepository.GetCompanyByIdAsync(request.CompanyId);

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
                    Title = "Company5Tab Approval",
                    Message = $"Company 5 Tab data approved for company '{companyEntity?.CompanyName}'. Current onboarding status: {currentOnboardingStatus}",
                    NotificationType = "Info",
                    IsRead = false,
                    IsDeleted = false,
                    // Notification is active only if onboarding is approved
                    IsActive = currentOnboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED
                };
                await _notificationsRepository.InsertNotificationAsync(notification);
            }

            // Corrected line to use double quotes for string literals and removed assignment to a variable (since the method returns Task, not a value)
            await _companiesRepository.CloneElixirTenantDatabaseAsync(
                _dbConfigSettings.TenantDbTemplate,
                $"{request.Company5TabDto.Company5TabCompanyDto.CompanyName}",
                _dbConfigSettings.ElasticPoolName
            );

            return true;
        }
        catch (Exception ex)
        {
            await transactionRepository.RollbackAsync();
            return false;
        }
    }
}
