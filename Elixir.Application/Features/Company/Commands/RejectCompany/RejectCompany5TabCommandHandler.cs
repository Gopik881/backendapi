using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ApproveCompanyOnBoardingStatus;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.CompositeWithdrawCompany5Tab;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;
using System.ComponentModel.Design;

namespace Elixir.Application.Features.Company.Commands.RejectCompany;
public record RejectCompany5TabCommand(int companyId, int UserId, string rejectionReason) : IRequest<bool>;

public class RejectCompany5TabCommandHandler : IRequestHandler<RejectCompany5TabCommand, bool>
{
    private readonly ICompany5TabOnboardingHistoryRepository _company5TabOnboardingHistory;
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    private readonly ICompaniesRepository _companiesRepository;
    private readonly INotificationsRepository _notificationsRepository;
    private readonly IMediator _mediator; // Add this field
    private readonly IUsersRepository _usersRepository;

    public RejectCompany5TabCommandHandler(ICompany5TabOnboardingHistoryRepository Company5TabOnboardingHistory,
        ICompanyOnboardingStatusRepository companyOnboardingStatusRepository,
        ICompaniesRepository companiesRepository,
        INotificationsRepository notificationsRepository,
        IMediator mediator,
        IUsersRepository usersRepository) // Add this parameter
    {
        _company5TabOnboardingHistory = Company5TabOnboardingHistory;
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
        _companiesRepository = companiesRepository;
        _notificationsRepository = notificationsRepository;
        _mediator = mediator; // Initialize the field
        _usersRepository = usersRepository;
    }

    public async Task<bool> Handle(RejectCompany5TabCommand request, CancellationToken cancellationToken)
    {
        bool IsEnable = false;
        // Check if onboarding status exists
        var onboardingStatus = await _companyOnboardingStatusRepository.GetCompanyOnBoardingStatus(request.companyId);
        if (string.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_APPROVED))
        {
            IsEnable = true;
            await _companiesRepository.UpdateCompanyUnderEditAsync(request.companyId, request.UserId, false);
            await _companiesRepository.UpdateCompanyLastUpdatedBy(request.companyId, request.UserId);
            var command = new CompositeWithDrawCompany5TabCommand(request.companyId, request.UserId);
            var result = await _mediator.Send(command); // Use the _mediator field
        }
        else if (string.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_PENDING))
        {
            IsEnable = false;
            await _companyOnboardingStatusRepository.UpdateOnboardingStatusAsync(request.companyId, request.UserId, AppConstants.ONBOARDING_STATUS_REJECTED);
            await _companiesRepository.UpdateCompanyHistoryLastUpdatedBy(request.companyId, request.UserId);

        }
        await _company5TabOnboardingHistory.Company5TabCreateOnboardingHistoryAsync(request.companyId, request.UserId, AppConstants.ONBOARDING_STATUS_REJECTED, request.rejectionReason, IsEnable);

        // Notification logic for Rejected Company
        var company = await _companiesRepository.GetCompanyByIdAsync(request.companyId);
        var accountManagerAndCheckerUserIds = await _usersRepository.GetAccountManagersAndCheckersUserIdsAsync(request.companyId);

        foreach (var userId in accountManagerAndCheckerUserIds)
        {
            var notification = new NotificationDto
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
                CompanyId = request.companyId,
                Title = "Company5Tab Rejection",
                Message = $"Company 5 Tab data rejected for company '{company?.CompanyName}'. Reason: {request.rejectionReason}. Current onboarding status: {onboardingStatus}",
                NotificationType = "Warning",
                IsRead = false,
                IsDeleted = false,
                IsActive = onboardingStatus == AppConstants.ONBOARDING_STATUS_REJECTED
            };
            await _notificationsRepository.InsertNotificationAsync(notification);
        }
        return true;
    }
}
