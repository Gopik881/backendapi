using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Company.Commands.RejectCompany;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.Account;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.Company;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.CompanyAdmin;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.ElixirUser;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.EscalationContact;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.ModuleMapping;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.ReportingToolLimit;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.WithDrawCompanyOnBoardingStatus;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.CompositeWithdrawCompany5Tab;

public record CompositeWithDrawCompany5TabCommand(int CompanyId, int UserId) : IRequest<bool>;

public class CompositeWithDrawCompany5TabCommandHandler : IRequestHandler<CompositeWithDrawCompany5TabCommand, bool>
{
    private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
    private readonly IMediator _mediator;
    private readonly INotificationsRepository _noticationsRespository;
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatus;
    private readonly ICompaniesRepository _compniesRepository;
    private readonly IUsersRepository _usersRepository;

    public CompositeWithDrawCompany5TabCommandHandler(
        Func<ITransactionRepository> transactionRepositoryFactory,
        IMediator mediator,
        INotificationsRepository noticationsRespository,
        ICompanyOnboardingStatusRepository company5TabOnboardingHistory,
        ICompaniesRepository companiesRepository,
        IUsersRepository usersRepository)
    {
        _transactionRepositoryFactory = transactionRepositoryFactory;
        _mediator = mediator;
        _noticationsRespository = noticationsRespository;
        _compniesRepository = companiesRepository;
        _companyOnboardingStatus = company5TabOnboardingHistory;
        _usersRepository = usersRepository;
    }

    public async Task<bool> Handle(CompositeWithDrawCompany5TabCommand request, CancellationToken cancellationToken)
    {
        using var transactionRepository = _transactionRepositoryFactory();
        await transactionRepository.BeginTransactionAsync();
        try
        {
            // Withdraw company data
            var companyDataCommand = new Company5TabWithDrawCompanyDataCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(companyDataCommand))
                throw new Exception("Failed to withdraw Company data.");

            // Withdraw account data
            var accountCommand = new Company5TabWithDrawAccountCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(accountCommand))
                throw new Exception("Failed to withdraw Account data.");

            // Withdraw company admin data
            var adminCommand = new Company5TabWithDrawCompanyAdminCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(adminCommand))
                throw new Exception("Failed to withdraw Company Admin data.");

            // Withdraw module mapping data
            var moduleMappingCommand = new Company5TabWithDrawModuleMappingCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(moduleMappingCommand))
                throw new Exception("Failed to withdraw Module Mapping data.");

            // Withdraw reporting tool limits data
            var reportingToolLimitsCommand = new Company5TabWithDrawReportingToolLimitCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(reportingToolLimitsCommand))
                throw new Exception("Failed to withdraw Reporting Tool Limits data.");

            // Withdraw escalation contacts data
            var escalationContactCommand = new Company5TabWithDrawEscalationContactsCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(escalationContactCommand))
                throw new Exception("Failed to withdraw Escalation Contacts data.");

            // Withdraw elixir users data
            var elixirUsersCommand = new Company5TabWithDrawElixirUserCommand(request.CompanyId, request.UserId);
            if (!await _mediator.Send(elixirUsersCommand))
                throw new Exception("Failed to withdraw Elixir Users data.");
            
            var WithDrawCompanyOnboardingStatus = new WithDrawCompany5TabOnboardingStatusCommand(request.CompanyId, request.UserId);
            if( !await _mediator.Send(WithDrawCompanyOnboardingStatus, cancellationToken))
                throw new Exception("Failed to update company onboarding status.");

            await transactionRepository.CommitAsync();

            // Notification logic for Withdrawn Company
            // Retrieve the company entity by its ID
            var companyEntity = await _compniesRepository.GetCompanyByIdAsync(request.CompanyId);

            // Get the current onboarding status for the company
            var onboardingStatus = await _companyOnboardingStatus.GetCompanyOnBoardingStatus(request.CompanyId);

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
                    Title = "Company5Tab Withdrawn",
                    Message = $"Company 5 Tab data withdrawn for company '{companyEntity?.CompanyName}'.",
                    NotificationType = "Info",
                    IsRead = false,
                    IsDeleted = false,
                    IsActive = onboardingStatus == AppConstants.ONBOARDING_STATUS_APPROVED
                };
                await _noticationsRespository.InsertNotificationAsync(notification);
            }
            return true;
        }
        catch (Exception)
        {
            await transactionRepository.RollbackAsync();
            return false;
        }
    }
}
