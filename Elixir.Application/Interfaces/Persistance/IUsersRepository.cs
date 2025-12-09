using Elixir.Application.Features.User.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IUsersRepository
{
    Task<UserLoginDto> GetUserByEmailAsync(string email, int emailHash);
    Task<bool> ExistsUserByEmailAsync(string email, int emailHash);
    Task<bool> UpdateUserPasswordAsync(string email, int emailHash, string newPasswordHash, string salt);
    Task<Tuple<List<NonAdminUserDto>, int>> GetFilteredNonAdminUsersAsync(string searchTerm, int pageNumber, int pageSize);
    Task<UserProfileDto> GetUserProfileAsync(string email, int emailHash);
    Task<bool> UpdateUserProfileAsync(UserProfileDto userProfile, int emailHash, bool IsSuperAdmin = false);
    Task<int> CreateUserAsync(UserProfileDto userDto, int emailHash);
    Task<UserProfileDto> GetUserProfileByUserIdAsync(int userId, bool? IsSuperUser = false);
    Task<bool> UpdateUserAsync(UserProfileDto userDto, int emailHash);
    Task<bool> DeleteUserAsync(int userId);
    Task<List<string>> GetUsersCriticalGroupAsync(int userId);
    Task<List<string>> GetAllUsersEmailAsync();
    Task<bool> BulkInsertUsersAsync(List<UserBulkUploadDto> users);
    Task<List<int>> GetAccountManagersAndCheckersUserIdsAsync(int companyId);

    Task<bool> UpdateUserSessionActiveTimeAsync(string email, int emailHash,DateTime userSessionActiveTime);
    Task<bool> UpdateUserLoginFailedAttemptTimeAsync(string email, int emailHash, DateTime? lastFailedAttemptTime, int? FailedLoginAttempts);

    Task<bool> SaveResetPasswordTokenAsync(int emailHash, string email, string token);

    Task<bool> SetResetPasswordTokenEmptyAsync(string emailId, string token);

    Task<bool> UpdateUserEmailHasPasswordAsync(string email, int existingemailHash, int newemailHash, string newPasswordHash, string salt);

    Task<bool> ValidateResetLinkToken(string token);
}