using Elixir.Application.Common.Constants;
using Elixir.Application.Features.User.Commands.ChangePassword;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;
using System.Text;


namespace Elixir.Application.Features.User.Commands.ResetPassword;

public record ResetPasswordCommand(int userId, ResetPasswordRequestDto resetPasswordRequestDto) : IRequest<ResetPasswordResponseDto>;
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, ResetPasswordResponseDto>
{
    IUsersRepository _usersRepository;
    IUserPasswordHistoryRepository _userPasswordHistoryRepository;
    ISystemPoliciesRepository _systemPoliciesRepository;
    ICryptoService _cryptoService;
    IMediator _mediator;
    INotificationsRepository _notificationsRepository;
    public ResetPasswordCommandHandler(IUsersRepository usersRepository, IUserPasswordHistoryRepository userPasswordHistoryRepository, ISystemPoliciesRepository systemPoliciesRepository, ICryptoService cryptoService, IMediator mediator, INotificationsRepository notificationsRepository)
    {
        _usersRepository = usersRepository;
        _userPasswordHistoryRepository = userPasswordHistoryRepository;
        _systemPoliciesRepository = systemPoliciesRepository;
        _cryptoService = cryptoService;
        _mediator = mediator;
        _notificationsRepository = notificationsRepository;
    }

    public async Task<ResetPasswordResponseDto> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        try
        {
            // If Token is null or empty return failure response
            if (string.IsNullOrEmpty(request.resetPasswordRequestDto.Token))
                return new ResetPasswordResponseDto() { Status = false, Message = AppConstants.ErrorCodes.RESET_TOKEN_INVALID };

            var tokenData = _cryptoService.DecryptPasswordResetData(request.resetPasswordRequestDto.Token);
            if (tokenData == null)
                return new ResetPasswordResponseDto { Status = false, Message = AppConstants.ErrorCodes.RESET_TOKEN_INVALID };
            if (tokenData.ExpiryDate <= DateTime.UtcNow)
                return new ResetPasswordResponseDto { Status = false, Message = AppConstants.ErrorCodes.RESET_TOKEN_INVALID };

            var userFromToken = await _usersRepository.GetUserByEmailAsync(tokenData.Email.Trim(), _cryptoService.GenerateIntegerHashForString(tokenData.Email));
            if (userFromToken == null)
                return new ResetPasswordResponseDto { Status = false, Message = "User does not exist." };

            var systemPolicy = await _systemPoliciesRepository.GetDefaultSystemPolicyAsync();
            if (systemPolicy == null)
                return new ResetPasswordResponseDto() { Status = false, Message = "System Policy not defined, unable to proceed." };

            var pwd = request.resetPasswordRequestDto.NewPassword?.Trim() ?? string.Empty;

            // Weak password blacklist        
            var weakPasswords = AppConstants.WeakPasswords;

            // Check if all special characters in the password are allowed by the policy
            bool allSpecialCharsAllowed = pwd
                .Where(ch => !char.IsLetterOrDigit(ch))
                .All(ch => (systemPolicy.SpecialCharactersAllowed ?? string.Empty).Contains(ch));


            bool isPwdValid =
                pwd.Length >= (systemPolicy.MinLength ?? 0) &&
                pwd.Length <= (systemPolicy.MaxLength ?? int.MaxValue) &&
                pwd.Count(char.IsUpper) >= (systemPolicy.NoOfUpperCase ?? 0) &&
                pwd.Count(char.IsLower) >= (systemPolicy.NoOfLowerCase ?? 0) &&
                pwd.Count(ch => (systemPolicy.SpecialCharactersAllowed ?? string.Empty).Contains(ch)) >= (systemPolicy.NoOfSpecialCharacters ?? 0) &&
                !weakPasswords.Contains(pwd);

            if (!isPwdValid || !allSpecialCharsAllowed)
            {
                var errors = new List<string>
            {
                "New password does not meet the policy requirements."
            };
                if (weakPasswords.Contains(pwd))
                {
                    errors.Add("The password you entered is too common or weak. Please choose a stronger password.");
                }
                return new ResetPasswordResponseDto()
                {
                    Status = false,
                    Message = "New password does not meet the policy requirements.",
                    Errors = errors
                };
            }

            #region Check the Historical Passwords 
            var previousPasswordData = await _userPasswordHistoryRepository.GetHistoricalPasswordDataForUserAsync(userFromToken.Id, systemPolicy.HistoricalPasswords ?? 1);
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
                    return new ResetPasswordResponseDto() { Status = false, Message = $"New password must be unique and not match any of the last {systemPolicy.HistoricalPasswords} passwords." };
                }
            }
            #endregion

            //Generate New Salt for any reset password request
            var newSalt = _cryptoService.GenerateSalt();
            var newPasswordHash = _cryptoService.HashPasswordWithSalt(Encoding.UTF8.GetBytes(pwd), Convert.FromBase64String(newSalt));
            //if (!await _userPasswordHistoryRepository.IsPasswordUniqueAsync(userFromToken.Id, Convert.ToBase64String(newPasswordHash), systemPolicy.HistoricalPasswords ?? 1))
            //    return new ResetPasswordResponseDto() { Status = false, Message = $"New password must be unique and not match any of the last {systemPolicy.HistoricalPasswords} passwords." };
            if (!await _usersRepository.UpdateUserPasswordAsync(userFromToken.Email, _cryptoService.GenerateIntegerHashForString(userFromToken.Email), Convert.ToBase64String(newPasswordHash), newSalt))
                return new ResetPasswordResponseDto() { Status = false, Message = "Failed to update user password." };
            if (!await _userPasswordHistoryRepository.CreatePasswordHistory(userFromToken.Id, Convert.ToBase64String(newPasswordHash), newSalt))
                return new ResetPasswordResponseDto() { Status = false, Message = "Failed to update password history." };
            await _userPasswordHistoryRepository.DeleteOldPasswordHistoryAsync(userFromToken.Id, systemPolicy.HistoricalPasswords ?? 1);

            await _usersRepository.SaveResetPasswordTokenAsync(_cryptoService.GenerateIntegerHashForString(userFromToken.Email), userFromToken.Email, null); //Clear the reset token after successful password reset
            return new ResetPasswordResponseDto() { Status = true, Message = "Password updated successfully." };
        }
        catch
        {
            throw new Exception(AppConstants.ErrorCodes.RESET_PASSWORD_FAILURE);
        }
       
    }
}
 