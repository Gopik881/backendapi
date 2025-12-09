using Elixir.Application.Common.Constants;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateCompany5TabOnboardingHistoryCommand(int CompanyId, int UserId) : IRequest<bool>;

public class CreateCompany5TabOnboardingHistoryCommandHandler : IRequestHandler<CreateCompany5TabOnboardingHistoryCommand, bool>
{
    private readonly ICompany5TabOnboardingHistoryRepository _companyOnboardingHistoryRepository;
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    private readonly ICompaniesRepository _companiesRepository;

    public CreateCompany5TabOnboardingHistoryCommandHandler(ICompany5TabOnboardingHistoryRepository companyOnboardingHistoryRepository,
        ICompanyOnboardingStatusRepository companyOnboardingStatusRepository,
        ICompaniesRepository companiesRepository)
    {
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
        _companyOnboardingHistoryRepository = companyOnboardingHistoryRepository;
        _companiesRepository = companiesRepository;
    }

    public async Task<bool> Handle(CreateCompany5TabOnboardingHistoryCommand request, CancellationToken cancellationToken)
    {
        // Check if onboarding status exists
        var onboardingStatus = await _companyOnboardingStatusRepository.GetCompanyOnBoardingStatus(request.CompanyId);
        bool? companyActiveStatus = await _companyOnboardingStatusRepository.GetCompanyActiveStatus(request.CompanyId);
        if (String.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_NEW) || String.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_REJECTED) || (String.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_APPROVED) && companyActiveStatus == false))
        {
            await _companyOnboardingStatusRepository.UpdateOnboardingStatusAsync(request.CompanyId, request.UserId, AppConstants.ONBOARDING_STATUS_PENDING);
            onboardingStatus = AppConstants.ONBOARDING_STATUS_PENDING;
            await _companyOnboardingHistoryRepository.Company5TabCreateOnboardingHistoryAsync(request.CompanyId, request.UserId, onboardingStatus, null, false);
        }
        else if (String.Equals(onboardingStatus, AppConstants.ONBOARDING_STATUS_APPROVED))
        {
            await _companiesRepository.UpdateCompanyUnderEditAsync(request.CompanyId, request.UserId, true);
            await _companiesRepository.UpdateCompanyLastUpdatedBy(request.CompanyId, request.UserId);
        }
        // Update onboarding status
        return true;
    }
}
