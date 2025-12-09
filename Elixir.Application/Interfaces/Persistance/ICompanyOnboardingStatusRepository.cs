using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface ICompanyOnboardingStatusRepository
{
    Task<bool> IsCompanyOnboardingStatusDataExistsAsync(int companyId);
    Task<string> GetCompanyOnBoardingStatus(int companyId);
    Task<bool> UpdateOnboardingStatusAsync(int companyId, int userId, string newStatus, bool isWithDraw = false);
    Task<Tuple<List<CompanyTMIOnBoardingSummaryDto>, int>> GetPagedTMIUsersCompaniesOnBoardingSummaryAsync(int userId, string searchTerm, int pageNumber, int pageSize, bool isDashboard = false);
    Task<Tuple<List<CompanyOnBoardingSummaryDto>, int>> GetPagedSuperAdminCompaniesOnBoardingSummaryAsync(int userId, bool IsSuperUser, string searchTerm, int pageNumber, int pageSize);
    Task<Tuple<List<CompanyOnBoardingSummaryDto>, int>> GetPagedDelegateAdminCompaniesOnBoardingSummaryAsync(int userId, string searchTerm, int pageNumber, int pageSize);
    Task<int> GetCompanyIdByClientIdAsync(int clientId);
    Task<bool?> GetCompanyActiveStatus(int companyId);
}
