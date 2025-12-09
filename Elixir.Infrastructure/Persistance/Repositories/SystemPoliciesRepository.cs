using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.SystemPolicies.DTOs;
using Elixir.Application.Common.Constants;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class SystemPoliciesRepository : ISystemPoliciesRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public SystemPoliciesRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<SystemPolicy> GetSystemPolicyByIdAsync(int systemPolicyId)
    {
        return await _dbContext.SystemPolicies
            .Where(sp => sp.Id == systemPolicyId && !sp.IsDeleted)
            .FirstOrDefaultAsync();
    }

    public async Task<SystemPolicyDto> GetDefaultSystemPolicyAsync()
    {
        return await _dbContext.SystemPolicies
            .Where(sp => sp.IsEnabled && !sp.IsDeleted)
            .Select(sp => new SystemPolicyDto
            {
                SystemPolicyId = sp.Id,
                FileSizeLimitMb = sp.FileSizeLimitMb,
                HistoricalPasswords = sp.HistoricalPasswords,
                LockInPeriodInMinutes = sp.LockInPeriodInMinutes,
                MaxLength = sp.MaxLength,
                MinLength = sp.MinLength,
                NoOfLowerCase = sp.NoOfLowerCase,
                NoOfSpecialCharacters = sp.NoOfSpecialCharacters,
                NoOfUpperCase = sp.NoOfUpperCase,
                PasswordValidityDays = sp.PasswordValidityDays,
                SessionTimeoutMinutes = sp.SessionTimeoutMinutes,
                SpecialCharactersAllowed = sp.SpecialCharactersAllowed,
                UnsuccessfulAttempts = sp.UnsuccessfulAttempts
            }).FirstOrDefaultAsync() ?? new SystemPolicyDto
            {
                SystemPolicyId = 0,
                FileSizeLimitMb = 0,
                HistoricalPasswords = 3,
                LockInPeriodInMinutes = 0,
                MaxLength = 0,
                MinLength = 0,
                NoOfLowerCase = 0,
                NoOfSpecialCharacters = 0,
                NoOfUpperCase = 0,
                PasswordValidityDays = 0,
                SessionTimeoutMinutes = AppConstants.SessionTimeoutInMinutes,
                SpecialCharactersAllowed = string.Empty,
                UnsuccessfulAttempts = 0
            };
    }

    public async Task<bool> UpdateSystemPolicyAsync(SystemPolicy systemPolicy)
    {
        try
        {
            if (systemPolicy == null) return false;
            _dbContext.SystemPolicies.Update(systemPolicy);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.PASSWORD_POLICY_NOT_FOUND);
        }
    }


}
