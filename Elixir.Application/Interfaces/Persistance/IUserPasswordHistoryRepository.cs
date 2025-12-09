using Elixir.Application.Features.User.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IUserPasswordHistoryRepository
{
    Task<bool> DeleteOldPasswordHistoryAsync(int userId, int historyLimit);
    Task<bool> CreatePasswordHistory(int userId, string newPasswordHash, string salt);
    //Task<bool> IsPasswordUniqueAsync(int userId, string newPasswordHash, int historyLimit);
    Task<List<UserPasswordHistoryDataDto>> GetHistoricalPasswordDataForUserAsync(int userId, int historyLimit);
}