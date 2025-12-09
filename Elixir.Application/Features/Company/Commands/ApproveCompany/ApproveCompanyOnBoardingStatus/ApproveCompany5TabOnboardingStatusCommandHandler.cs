using Elixir.Application.Common.Constants;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.ApproveCompanyOnBoardingStatus;

public record ApproveCompany5TabOnboardingHistoryCommand(int CompanyId, int UserId) : IRequest<bool>;

public class ApproveCompany5TabOnboardingHistoryCommandHandler : IRequestHandler<ApproveCompany5TabOnboardingHistoryCommand, bool>
{
    private readonly ICompany5TabOnboardingHistoryRepository _companyOnboardingHistoryRepository;
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    private readonly ICompaniesRepository _companiesRepository;
    private readonly ICompanyHistoryRepository _companyHistoryRepository;
    public ApproveCompany5TabOnboardingHistoryCommandHandler(ICompany5TabOnboardingHistoryRepository companyOnboardingHistoryRepository,
        ICompanyOnboardingStatusRepository companyOnboardingStatusRepository,
        ICompaniesRepository companiesRepository,
        ICompanyHistoryRepository companyHistoryRepository)
    {
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
        _companyOnboardingHistoryRepository = companyOnboardingHistoryRepository;
        _companiesRepository = companiesRepository;
        _companyHistoryRepository = companyHistoryRepository;
    }

    public async Task<bool> Handle(ApproveCompany5TabOnboardingHistoryCommand request, CancellationToken cancellationToken)
    {
        bool? IsEnabled = false;
        //int userId = 0;
        //var userIdfromCompanyHistory = await _companyHistoryRepository.GetCompanyHistoryUserIdByCompanyId(request.CompanyId);
        // Check if onboarding status exists
        var onboardingStatus = await _companyOnboardingStatusRepository.GetCompanyOnBoardingStatus(request.CompanyId);
        if (string.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_APPROVED))
        {
            IsEnabled = true;
            //userId = userIdfromCompanyHistory;
            await _companiesRepository.UpdateCompanyUnderEditAsync(request.CompanyId, request.UserId, false);
        }
        else if (string.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_PENDING))
        {
            await _companyOnboardingStatusRepository.UpdateOnboardingStatusAsync(request.CompanyId, request.UserId, AppConstants.ONBOARDING_STATUS_APPROVED);
            await _companiesRepository.UpdateCompanyLastUpdatedBy(request.CompanyId, request.UserId);
            onboardingStatus = AppConstants.ONBOARDING_STATUS_APPROVED;
        }
        
        // Update onboarding status
        return await _companyOnboardingHistoryRepository.Company5TabCreateOnboardingHistoryAsync(request.CompanyId, request.UserId, onboardingStatus, "", IsEnabled);
    }
}
