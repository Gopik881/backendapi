using Elixir.Application.Common.Constants;
using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.SingleSession.Interface;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;
using System.Text;

namespace Elixir.Application.Features.User.Commands.Login;

public record LoginCommand(LoginRequestDto LoginReqDto) : IRequest<LoginResponseDto>;
public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUsersRepository _usersRepository;
    private readonly ISuperUsersRepository _superUsersRepository;
    private readonly ISystemPoliciesRepository _systemPoliciesRepository;
    private readonly IUserGroupMappingsRepository _userGroupMappingsRepository;
    private readonly ICryptoService _cryptoService;
    private readonly IEmailService _emailService; // Uncomment if email service is needed for sending notifications
    private readonly ISessionService _sessionService; // Assuming you have a session service to handle session invalidation
    public LoginCommandHandler(IUsersRepository usersRepository, ISystemPoliciesRepository systemPoliciesRepository, ICryptoService cryptoService,
        IUserGroupMappingsRepository userGroupMappingsRepository, ISuperUsersRepository superUsersRepository, IEmailService emailService, ISessionService sessionService)
    {
        _usersRepository = usersRepository;
        _systemPoliciesRepository = systemPoliciesRepository;
        _cryptoService = cryptoService;
        _userGroupMappingsRepository = userGroupMappingsRepository;
        _superUsersRepository = superUsersRepository;
        _emailService = emailService;
        _sessionService = sessionService;
    }
    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        if (!string.Equals(request.LoginReqDto.CompanyCode, "Tmi", StringComparison.OrdinalIgnoreCase))
        {
            return new LoginResponseDto { Success = false, Message = "Invalid credentials." };
        }

        bool IsSuperUser = false;
        var systemPolicy = await _systemPoliciesRepository.GetDefaultSystemPolicyAsync();

        request.LoginReqDto.UserName = request.LoginReqDto.UserName.Trim().ToLowerInvariant();
        var user = await _usersRepository.GetUserByEmailAsync(
            request.LoginReqDto.UserName,
            _cryptoService.GenerateIntegerHashForString(request.LoginReqDto.UserName)
        );
        if(user != null && user.isSuperUser == true)
        {
            IsSuperUser = true;
        }
        if (user != null && user.PasswordHash == null)
            return new LoginResponseDto { Success = false, Message = "Invalid credentials." };

        if (user == null)
        {
            user = await _superUsersRepository.GetUserByEmailAsync(
                request.LoginReqDto.UserName,
                _cryptoService.GenerateIntegerHashForString(request.LoginReqDto.UserName)
            );

            if (user != null)
                IsSuperUser = true;
            else
                return new LoginResponseDto { Success = false, Message = "Invalid Credentials" };
        }

        if (user.LastFailedAttempt.HasValue && user.LastFailedAttempt > DateTime.UtcNow)
        {
            return new LoginResponseDto
            {
                Success = false,
                Message = $"Account is locked. Try again after {(user.LastFailedAttempt.Value - DateTime.UtcNow):hh\\:mm\\:ss}."
            };
        }

        var recreatedPasswordHash = _cryptoService.HashPasswordWithSalt(
            Encoding.UTF8.GetBytes(request.LoginReqDto.Password),
            Convert.FromBase64String(user.Salt)
        );

        if (!_cryptoService.FixedTimeEquals(Convert.FromBase64String(user.PasswordHash), recreatedPasswordHash))
        {
            user.FailedLoginAttempts = (user.FailedLoginAttempts ?? 0) + 1;

            if (systemPolicy.UnsuccessfulAttempts.HasValue && user.FailedLoginAttempts >= systemPolicy.UnsuccessfulAttempts)
            {
                user.LastFailedAttempt = DateTime.UtcNow.AddMinutes(systemPolicy.LockInPeriodInMinutes ?? 15);
            }

            await _usersRepository.UpdateUserLoginFailedAttemptTimeAsync(
                user.Email,
                _cryptoService.GenerateIntegerHashForString(user.Email),
                user.LastFailedAttempt ?? DateTime.UtcNow,
                user.FailedLoginAttempts
            );

            return new LoginResponseDto { Success = false, Message = "Invalid credentials." };
        }

        // SINGLE ACTIVE SESSION LOGIC
        // Invalidate existing sessions
        await _sessionService.InvalidateSessions(user.Id);

        // Generate new SessionId
        var sessionId = Guid.NewGuid();

        // Prepare claims
        var otherClaims = new Dictionary<string, string>
        {
            { AppConstants.USER_ID, user.Id.ToString() },
            { "SessionId", sessionId.ToString() }, // <-- add SessionId here
            { AppConstants.IS_SUPER_ADMIN, IsSuperUser ? AppConstants.IS_SUPER_ADMIN_TRUE : AppConstants.IS_SUPER_ADMIN_FALSE }
        };

        // Generate token
        var userToken = _cryptoService.GenerateEncryptedToken(user.Id.ToString(), user.Email, otherClaims);
        if (userToken == null)
            return new LoginResponseDto { Success = false, Message = "Failed to generate user token." };

        // Save new session
        await _sessionService.CreateSession(user.Id, userToken.AccessToken, sessionId);

        //// Send email if previous session existed
        //if (await _sessionService.HadPreviousSession(user.Id))
        //{
        //    _ = Task.Run(async () =>
        //    {
        //        await _emailService.SendEmailAsync(new EmailRequestDto
        //        {
        //            To = user.Email,
        //            Subject = "Login Attempt Notification",
        //            HtmlBody = $"Dear {user.FirstName},<br/><br/>Your previous session has been terminated due to another login for your account on {DateTime.UtcNow}. If this was not you, please secure your account immediately.<br/><br/>Thank you,<br/>Elixir Team",
        //        });
        //    });
        //}

        // Save session expiry time for super or normal user
        if (IsSuperUser)
        {
            var saveStatus = await _superUsersRepository.UpdateSuperUserSessionActiveTimeAsync(
                user.Email,
                _cryptoService.GenerateIntegerHashForString(user.Email),
                userToken.AccessTokenExpiry
            );

            if (!saveStatus)
                return new LoginResponseDto { Success = false, Message = "Failed to create user session." };
        }
        else
        {
            var saveStatus = await _usersRepository.UpdateUserSessionActiveTimeAsync(
                user.Email,
                _cryptoService.GenerateIntegerHashForString(user.Email),
                userToken.AccessTokenExpiry
            );

            if (!saveStatus)
                return new LoginResponseDto { Success = false, Message = "Failed to create user session." };
        }

        await _usersRepository.UpdateUserLoginFailedAttemptTimeAsync(user.Email, _cryptoService.GenerateIntegerHashForString(user.Email), null, 0);

        // Check if all special characters in the password are allowed by the policy
        bool allSpecialCharsAllowed = request.LoginReqDto.Password
            .Where(ch => !char.IsLetterOrDigit(ch))
            .All(ch => (systemPolicy.SpecialCharactersAllowed ?? string.Empty).Contains(ch));

        bool isPwdValid =
              request.LoginReqDto.Password.Length >= (systemPolicy.MinLength ?? 0) &&
              request.LoginReqDto.Password.Length <= (systemPolicy.MaxLength ?? int.MaxValue) &&
              request.LoginReqDto.Password.Count(char.IsUpper) >= (systemPolicy.NoOfUpperCase ?? 0) &&
              request.LoginReqDto.Password.Count(char.IsLower) >= (systemPolicy.NoOfLowerCase ?? 0);

        return new LoginResponseDto
        {
            Success = true,
            UserMappedToUserGroup = IsSuperUser ? string.Empty : await _userGroupMappingsRepository.GetUserMappedToUserGroupWithHighestPrivelageAsync(user.Id),
            IsSuperAdmin = IsSuperUser,
            UserId = user.Id,
            Token = userToken.AccessToken,
            FullName = $"{user.FirstName} {user.LastName}",
            UserProfilePicture = user.ProfilePicture,
            SessionTimeout = systemPolicy.SessionTimeoutMinutes ?? AppConstants.SessionTimeoutInMinutes,
            IsPolicyChanged = !(allSpecialCharsAllowed && isPwdValid)
        };
    }

}
