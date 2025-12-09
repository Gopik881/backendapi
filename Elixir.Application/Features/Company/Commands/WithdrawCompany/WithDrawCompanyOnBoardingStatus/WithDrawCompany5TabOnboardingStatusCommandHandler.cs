using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.WithDrawCompanyOnBoardingStatus;

public record WithDrawCompany5TabOnboardingStatusCommand(int CompanyId, int UserId) : IRequest<bool>;

public class WithDrawCompany5TabOnboardingStatusCommandHandler : IRequestHandler<WithDrawCompany5TabOnboardingStatusCommand, bool>
{
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    private readonly ICompaniesRepository _companiesRepository;
    private readonly INotificationsRepository _notificationsRepository;
    private readonly IUsersRepository _usersRepository;

    public WithDrawCompany5TabOnboardingStatusCommandHandler(ICompanyOnboardingStatusRepository companyOnboardingStatusRepository,
        ICompaniesRepository companiesRepository,
        INotificationsRepository notificationsRepository,
        IUsersRepository usersRepository)
    {
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
        _companiesRepository = companiesRepository;
        _notificationsRepository = notificationsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<bool> Handle(WithDrawCompany5TabOnboardingStatusCommand request, CancellationToken cancellationToken)
    {
        bool isEnable = false;
        var onboardingStatus = await _companyOnboardingStatusRepository.GetCompanyOnBoardingStatus(request.CompanyId);
        // 2. Get company's IsEnable/active status from companies table (repository returns bool?)
        var companyIsEnableNullable = await _companiesRepository.GetCompanyByIdAsync(request.CompanyId);
        bool companyIsEnable = companyIsEnableNullable?.IsEnabled ?? false;

        if (string.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_PENDING) && !companyIsEnable)
        {
            isEnable = false;
            await _companyOnboardingStatusRepository.UpdateOnboardingStatusAsync(request.CompanyId, request.UserId, AppConstants.ONBOARDING_STATUS_NEW, true);
        }
        else if (string.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_APPROVED) && companyIsEnable)
        {
            isEnable = true;
            await _companiesRepository.UpdateCompanyUnderEditAsync(request.CompanyId, request.UserId, false);
        }

        var company = await _companiesRepository.GetCompanyByIdAsync(request.CompanyId);
        var accountManagerAndCheckerUserIds = await _usersRepository.GetAccountManagersAndCheckersUserIdsAsync(request.CompanyId);

        foreach (var userId in accountManagerAndCheckerUserIds)
        {
            var notification = new NotificationDto
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                UserId = userId,
                CompanyId = request.CompanyId,
                Title = "Company5Tab Withdrawn",
                Message = $"Company 5 Tab data withdrawn for company '{company?.CompanyName}'. Previous onboarding status: {onboardingStatus}",
                NotificationType = "Warning",
                IsRead = false,
                IsDeleted = false,
                IsActive = onboardingStatus == AppConstants.ONBOARDING_STATUS_NEW
            };
            await _notificationsRepository.InsertNotificationAsync(notification);
        }

        return true;
    }
}
