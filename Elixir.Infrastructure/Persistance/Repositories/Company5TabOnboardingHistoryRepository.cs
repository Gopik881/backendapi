using Elixir.Infrastructure.Data;
using Elixir.Domain.Entities;
using Elixir.Application.Interfaces.Persistance;
using Microsoft.EntityFrameworkCore;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Common.Enums;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class Company5TabOnboardingHistoryRepository : ICompany5TabOnboardingHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public Company5TabOnboardingHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> Company5TabCreateOnboardingHistoryAsync(int companyId, int userId, string onBoardingStatus, string? rejectionReason = null, bool? IsEnabled = false)
    {
        var latestVersion = await _dbContext.CompanyHistories
            .Where(ch => ch.CompanyId == companyId)
            .OrderByDescending(ch => ch.Version)
            .Select(ch => (int?)ch.Version)
            .FirstOrDefaultAsync();

        var onboardingHistory = new Company5TabOnboardingHistory
        {
            CompanyId = companyId,
            UserId = userId,
            Status = onBoardingStatus,
            Reason = rejectionReason,
            IsEnabled = IsEnabled ?? false,
            CreatedBy = latestVersion
        };
        await _dbContext.Company5TabOnboardingHistories.AddAsync(onboardingHistory);
        return await _dbContext.SaveChangesAsync() > 0;
    }
    public async Task<Company5TabOnboardingHistory?> Company5TabHistoryByVersionNumberAsync(int companyId, int versionNumber)
    {
        return await _dbContext.Company5TabOnboardingHistories
            .Where(h => h.CompanyId == companyId && h.Id == versionNumber)
            .FirstOrDefaultAsync();
    }
    public async Task<IEnumerable<Company5TabOnboardingHistoryDto>> GetCompany5TabOnboardingHistoryByCompanyIdAsync(int companyId, bool? IsRead = false)
    {
        var query =
            from history in _dbContext.Company5TabOnboardingHistories
            where history.CompanyId == companyId
            orderby history.CreatedAt descending
            select history.UserId == (int)Roles.SuperAdmin
                ? new Company5TabOnboardingHistoryDto
                {
                    companyId = history.CompanyId ?? 0,
                    UserId = history.UserId ?? 0,
                    FirstName = (from su in _dbContext.SuperUsers
                                 where su.Id == history.UserId
                                 select su.FirstName + " " + su.LastName).FirstOrDefault() ?? string.Empty,
                    Status = history.Status,
                    isRead = history.IsEnabled,
                    Reason = history.Reason,
                    LatestUpdate = history.CreatedAt
                }
                : new Company5TabOnboardingHistoryDto
                {
                    companyId = history.CompanyId ?? 0,
                    UserId = history.UserId ?? 0,
                    FirstName = (from user in _dbContext.Users
                                 where user.Id == history.UserId
                                 select user.FirstName + " " + user.LastName).FirstOrDefault() ?? string.Empty,
                    Status = history.Status,
                    isRead = history.IsEnabled,
                    Reason = history.Reason,
                    LatestUpdate = history.CreatedAt,
                    createdBy = history.CreatedBy ?? 0

                };

        return await query.ToListAsync();
    }
    

}
