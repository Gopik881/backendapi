using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface ICompany5TabOnboardingHistoryRepository
{
    //Task AddAsync(Company5TabOnboardingHistory entity);
    //Task DeleteAsync(int id);
    //Task<IEnumerable<Company5TabOnboardingHistory>> GetAllAsync();
    //Task<Company5TabOnboardingHistory> GetByIdAsync(int id);
    //Task UpdateAsync(Company5TabOnboardingHistory entity);

    Task<bool> Company5TabCreateOnboardingHistoryAsync(int userId, int companyId, string onBoardingStatus, string? rejectionReason = null, bool? IsEnabled = false);
    Task<IEnumerable<Company5TabOnboardingHistoryDto>> GetCompany5TabOnboardingHistoryByCompanyIdAsync(int companyId, bool? IsRead = false);
}