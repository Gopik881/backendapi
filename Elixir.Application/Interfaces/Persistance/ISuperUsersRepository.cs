using Elixir.Application.Features.User.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface ISuperUsersRepository
{
    Task<UserLoginDto> GetUserByEmailAsync(string email, int emailHash);
    Task<UserProfileDto> GetUserProfileAsync(string email, int emailHash);
    Task<bool> UpdateSuperUserSessionActiveTimeAsync(string email, int emailHash, DateTime userSuperSessionActiveTime);
    Task<string?> GetSuperUserEmailAsync(int superUserId);

}
