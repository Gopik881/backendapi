using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class UserPasswordHistoryRepository : IUserPasswordHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public UserPasswordHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> CreatePasswordHistory(int userId, string newPasswordHash, string salt)
    {
        var passwordHistory = new UserPasswordHistory
        {
            UserId = userId,
            PasswordHash = newPasswordHash,
            Salt = salt
        };
        await _dbContext.UserPasswordHistories.AddAsync(passwordHistory);
        await _dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteOldPasswordHistoryAsync(int userId, int historyLimit)
    {
        var oldRecords = await _dbContext.UserPasswordHistories.Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt).Skip(historyLimit).ToListAsync();
        _dbContext.UserPasswordHistories.RemoveRange(oldRecords);
        var result = await _dbContext.SaveChangesAsync();
        return result > 0 ? true : false;
    }
    //public async Task<bool> IsPasswordUniqueAsync(int userId, string newPasswordHash, int historyLimit)
    //{
    //    var previousPasswords = await _dbContext.UserPasswordHistories.Where(p => p.UserId == userId)
    //        .OrderByDescending(p => p.CreatedAt).Take(historyLimit).Select(p => p.PasswordHash).ToListAsync();
    //    return !previousPasswords.Contains(newPasswordHash);
    //}

    public async Task<List<UserPasswordHistoryDataDto>> GetHistoricalPasswordDataForUserAsync(int userId, int historyLimit)
    {
        var previousData = await _dbContext.UserPasswordHistories.Where(p => p.UserId == userId)
           .OrderByDescending(p => p.CreatedAt).Take(historyLimit).Select(p => new UserPasswordHistoryDataDto
           {
               PasswordHash = p.PasswordHash,
               Salt = p.Salt
           }).ToListAsync();
        return previousData;
    }
}
