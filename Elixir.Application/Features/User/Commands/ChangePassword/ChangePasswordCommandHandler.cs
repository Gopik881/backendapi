using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;
using System.Text;


namespace Elixir.Application.Features.User.Commands.ChangePassword;

public record ChangePasswordCommand(ChangePasswordRequestDto ChangePasswordRequestDto) : IRequest<ChangePasswordResponseDto>;
public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, ChangePasswordResponseDto>
{
    IUsersRepository _usersRepository;
    IUserPasswordHistoryRepository _userPasswordHistoryRepository;
    ISystemPoliciesRepository _systemPoliciesRepository;
    ICryptoService _cryptoService;
    INotificationsRepository _notificationsRepository;
    public ChangePasswordCommandHandler(IUsersRepository usersRepository, IUserPasswordHistoryRepository userPasswordHistoryRepository, ISystemPoliciesRepository systemPoliciesRepository, ICryptoService cryptoService, INotificationsRepository notificationsRepository)
    {
        _usersRepository = usersRepository;
        _userPasswordHistoryRepository = userPasswordHistoryRepository;
        _systemPoliciesRepository = systemPoliciesRepository;
        _cryptoService = cryptoService;
        _notificationsRepository = notificationsRepository;
    }

    public async Task<ChangePasswordResponseDto> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {

        var user = await _usersRepository.GetUserByEmailAsync(request.ChangePasswordRequestDto.UserName, _cryptoService.GenerateIntegerHashForString(request.ChangePasswordRequestDto.UserName));
        if (user == null) return new ChangePasswordResponseDto() { Status = false, Message = "User does not exist." };

        //Recreate the password hash from the old password and salt and check if this matching
        var recreatedPasswordHash = _cryptoService.HashPasswordWithSalt(Encoding.UTF8.GetBytes(request.ChangePasswordRequestDto.OldPassword), Convert.FromBase64String(user.Salt));
        if (!_cryptoService.FixedTimeEquals(Convert.FromBase64String(user.PasswordHash), recreatedPasswordHash))
        {
            return new ChangePasswordResponseDto
            {
                Status = false,
                Message = "Invalid credentials."
            };
        }

        var systemPolicy = await _systemPoliciesRepository.GetDefaultSystemPolicyAsync();
        if (systemPolicy == null) return new ChangePasswordResponseDto() { Status = false, Message = "System Policy not defined, unable to proceed." };



        var pwd = request.ChangePasswordRequestDto.NewPassword;
        bool allSpecialCharsAllowed = pwd
           .Where(ch => !char.IsLetterOrDigit(ch))
           .All(ch => (systemPolicy.SpecialCharactersAllowed ?? string.Empty).Contains(ch));
        bool isPwdValid =
            pwd.Length >= (systemPolicy.MinLength ?? 0) &&
            pwd.Length <= (systemPolicy.MaxLength ?? int.MaxValue) &&
            pwd.Count(char.IsUpper) >= (systemPolicy.NoOfUpperCase ?? 0) &&
            pwd.Count(char.IsLower) >= (systemPolicy.NoOfLowerCase ?? 0) &&
            pwd.Count(ch => (systemPolicy.SpecialCharactersAllowed ?? string.Empty).Contains(ch)) >= (systemPolicy.NoOfSpecialCharacters ?? 0);

        if (!isPwdValid || !allSpecialCharsAllowed)
        {
            return new ChangePasswordResponseDto()
            {
                Status = false,
                Message = "New password does not meet the policy requirements.",
                Errors = new List<string>
                {
                    "New password does not meet the policy requirements."
                }
            };
        }

        #region Check the Historical Passwords 
        var previousPasswordData = await _userPasswordHistoryRepository.GetHistoricalPasswordDataForUserAsync(user.Id, systemPolicy.HistoricalPasswords ?? 1);
        foreach (var entry in previousPasswordData)
        {
            // Rehash the new password using each historical salt to check for reuse.
            // This step is essential since a new salt is generated for every password change.
            // Direct hash comparison won't work because each hash is a combination of the password and its unique salt.
            // Reusing the old salt allows us to detect if the user is attempting to set a previously used password.
            var oldSalt = Convert.FromBase64String(entry.Salt);
            var oldHash = Convert.FromBase64String(entry.PasswordHash);

            var newHashWithOldSalt = _cryptoService.HashPasswordWithSalt(
                Encoding.UTF8.GetBytes(pwd),
                oldSalt
            );

            if (_cryptoService.FixedTimeEquals(newHashWithOldSalt, oldHash))
            {
                // Early exit once a match is found
                return new ChangePasswordResponseDto() { Status = false, Message = $"New password must be unique and not match any of the last {systemPolicy.HistoricalPasswords} passwords." };
            }
        }
        #endregion



        var newSalt = _cryptoService.GenerateSalt();
        var newPasswordHash = _cryptoService.HashPasswordWithSalt(Encoding.UTF8.GetBytes(pwd), Convert.FromBase64String(newSalt));

        if (!await _usersRepository.UpdateUserPasswordAsync(user.Email, _cryptoService.GenerateIntegerHashForString(user.Email), Convert.ToBase64String(newPasswordHash), newSalt))
            return new ChangePasswordResponseDto() { Status = false, Message = AppConstants.ErrorCodes.PASSWORD_CHANGE_FAILED };

        if (!await _userPasswordHistoryRepository.CreatePasswordHistory(user.Id, Convert.ToBase64String(newPasswordHash), newSalt))
            return new ChangePasswordResponseDto() { Status = false, Message = AppConstants.ErrorCodes.PASSWORD_CHANGE_FAILED };

        await _userPasswordHistoryRepository.DeleteOldPasswordHistoryAsync(user.Id, systemPolicy.HistoricalPasswords ?? 1);

        // After successful password update, send notification to user
        var notification = new NotificationDto
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = user.Id,
            UpdatedBy = user.Id,
            Title = "Password Changed",
            Message = $"Your password has been changed.",
            NotificationType = "Info",
            IsRead = false,
            IsDeleted = false,
            UserId = user.Id,
            IsActive = false
        };
        await _notificationsRepository.InsertNotificationAsync(notification);
        return new ChangePasswordResponseDto() { Status = true, Message = "Password updated successfully." };
    }
}
