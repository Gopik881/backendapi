using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.SystemPolicies.DTOs;
using Elixir.Application.Features.User.Commands.ChangePassword;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using Moq;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class ChangePasswordCommandHandlerTests
{
    private readonly Mock<IUsersRepository> _usersRepository = new();
    private readonly Mock<IUserPasswordHistoryRepository> _userPasswordHistoryRepository = new();
    private readonly Mock<ISystemPoliciesRepository> _systemPoliciesRepository = new();
    private readonly Mock<ICryptoService> _cryptoService = new();
    private readonly Mock<INotificationsRepository> _notificationsRepository = new();

    private ChangePasswordCommandHandler CreateHandler() =>
        new ChangePasswordCommandHandler(
            _usersRepository.Object,
            _userPasswordHistoryRepository.Object,
            _systemPoliciesRepository.Object,
            _cryptoService.Object,
            _notificationsRepository.Object);

    private ChangePasswordRequestDto ValidRequestDto =>
        new ChangePasswordRequestDto
        {
            UserName = "user@example.com",
            OldPassword = "OldPassword1!",
            NewPassword = "NewPassword1!"
        };

    private SystemPolicy ValidSystemPolicy =>
        new SystemPolicy
        {
            MinLength = 8,
            MaxLength = 20,
            NoOfUpperCase = 1,
            NoOfLowerCase = 1,
            NoOfSpecialCharacters = 1,
            SpecialCharactersAllowed = "!@#",
            HistoricalPasswords = 2
        };

    [Fact]
    public async Task Handle_UserDoesNotExist_ReturnsError()
    {
        _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync((UserLoginDto?)null);

        var handler = CreateHandler();
        var result = await handler.Handle(new ChangePasswordCommand(ValidRequestDto), CancellationToken.None);

        Assert.False(result.Status);
        Assert.Equal("User does not exist.", result.Message);
    }

    [Fact]
    public async Task Handle_InvalidOldPassword_ReturnsError()
    {
        var user = new UserLoginDto { PasswordHash = Convert.ToBase64String(new byte[] { 1, 2, 3 }), Salt = Convert.ToBase64String(new byte[] { 4, 5, 6 }) };
        _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(user);
        _cryptoService.Setup(x => x.HashPasswordWithSalt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(new byte[] { 7, 8, 9 });
        _cryptoService.Setup(x => x.FixedTimeEquals(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(false);

        var handler = CreateHandler();
        var result = await handler.Handle(new ChangePasswordCommand(ValidRequestDto), CancellationToken.None);

        Assert.False(result.Status);
        Assert.Equal("Invalid credentials.", result.Message);
    }

    //[Fact]
    //public async Task Handle_SystemPolicyNotDefined_ReturnsError()
    //{
    //    var user = new UserLoginDto { PasswordHash = Convert.ToBase64String(new byte[] { 1 }), Salt = Convert.ToBase64String(new byte[] { 2 }) };
    //    _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
    //        .ReturnsAsync(user);
    //    _cryptoService.Setup(x => x.HashPasswordWithSalt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
    //        .Returns(new byte[] { 1 });
    //    _cryptoService.Setup(x => x.FixedTimeEquals(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
    //        .Returns(true);
    //    _systemPoliciesRepository.Setup(x => x.GetDefaultSystemPolicyAsync())
    //        .ReturnsAsync(new SystemPolicyDto
    //        {
    //            MinLength = ValidSystemPolicy.MinLength,
    //            MaxLength = ValidSystemPolicy.MaxLength,
    //            NoOfUpperCase = ValidSystemPolicy.NoOfUpperCase,
    //            NoOfLowerCase = ValidSystemPolicy.NoOfLowerCase,
    //            NoOfSpecialCharacters = ValidSystemPolicy.NoOfSpecialCharacters,
    //            SpecialCharactersAllowed = ValidSystemPolicy.SpecialCharactersAllowed,
    //            HistoricalPasswords = ValidSystemPolicy.HistoricalPasswords
    //        });

    //    var handler = CreateHandler();
    //    var result = await handler.Handle(new ChangePasswordCommand(ValidRequestDto), CancellationToken.None);

    //    Assert.False(result.Status);
    //    Assert.Equal("System Policy not defined, unable to proceed.", result.Message);
    //}

    [Theory]
    [InlineData("short", "New password does not meet the policy requirements.")]
    [InlineData("NoSpecialChar1", "New password does not meet the policy requirements.")]
    [InlineData("nouppercase1!", "New password does not meet the policy requirements.")]
    [InlineData("NOLOWERCASE1!", "New password does not meet the policy requirements.")]
    [InlineData("NewPassword1$", "New password does not meet the policy requirements.")] // $ not allowed
    public async Task Handle_NewPasswordPolicyViolation_ReturnsError(string newPassword, string expectedMessage)
    {
        var user = new UserLoginDto { PasswordHash = Convert.ToBase64String(new byte[] { 1 }), Salt = Convert.ToBase64String(new byte[] { 2 }) };
        _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(user);
        _cryptoService.Setup(x => x.HashPasswordWithSalt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(new byte[] { 1 });
        _cryptoService.Setup(x => x.FixedTimeEquals(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(true);
        _systemPoliciesRepository.Setup(x => x.GetDefaultSystemPolicyAsync())
            .ReturnsAsync(new SystemPolicyDto
            {
                MinLength = ValidSystemPolicy.MinLength,
                MaxLength = ValidSystemPolicy.MaxLength,
                NoOfUpperCase = ValidSystemPolicy.NoOfUpperCase,
                NoOfLowerCase = ValidSystemPolicy.NoOfLowerCase,
                NoOfSpecialCharacters = ValidSystemPolicy.NoOfSpecialCharacters,
                SpecialCharactersAllowed = ValidSystemPolicy.SpecialCharactersAllowed,
                HistoricalPasswords = ValidSystemPolicy.HistoricalPasswords
            });

        var handler = CreateHandler();
        var requestDto = new ChangePasswordRequestDto
        {
            UserName = "user@example.com",
            OldPassword = "OldPassword1!",
            NewPassword = newPassword
        };
        var result = await handler.Handle(new ChangePasswordCommand(requestDto), CancellationToken.None);

        Assert.False(result.Status);
        Assert.Equal(expectedMessage, result.Message);
        Assert.Contains(expectedMessage, result.Errors);
    }

    [Fact]
    public async Task Handle_NewPasswordMatchesHistorical_ReturnsError()
    {
        var user = new UserLoginDto { PasswordHash = Convert.ToBase64String(new byte[] { 1 }), Salt = Convert.ToBase64String(new byte[] { 2 }), Email = "user@example.com", Id = 1 };
        _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(user);
        _cryptoService.Setup(x => x.HashPasswordWithSalt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(new byte[] { 1 });
        _cryptoService.Setup(x => x.FixedTimeEquals(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(true);
        _systemPoliciesRepository.Setup(x => x.GetDefaultSystemPolicyAsync())
           .ReturnsAsync(new SystemPolicyDto
           {
               MinLength = ValidSystemPolicy.MinLength,
               MaxLength = ValidSystemPolicy.MaxLength,
               NoOfUpperCase = ValidSystemPolicy.NoOfUpperCase,
               NoOfLowerCase = ValidSystemPolicy.NoOfLowerCase,
               NoOfSpecialCharacters = ValidSystemPolicy.NoOfSpecialCharacters,
               SpecialCharactersAllowed = ValidSystemPolicy.SpecialCharactersAllowed,
               HistoricalPasswords = ValidSystemPolicy.HistoricalPasswords
           });

        var history = new List<UserPasswordHistoryDataDto>
        {
            new UserPasswordHistoryDataDto { PasswordHash = Convert.ToBase64String(new byte[] { 10 }), Salt = Convert.ToBase64String(new byte[] { 20 }) }
        };
        _userPasswordHistoryRepository.Setup(x => x.GetHistoricalPasswordDataForUserAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(history);
        _cryptoService.Setup(x => x.HashPasswordWithSalt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(new byte[] { 10 });
        _cryptoService.Setup(x => x.FixedTimeEquals(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(true);

        var handler = CreateHandler();
        var result = await handler.Handle(new ChangePasswordCommand(ValidRequestDto), CancellationToken.None);

        Assert.False(result.Status);
        Assert.Contains("must be unique", result.Message);
    }

    [Fact]
    public async Task Handle_UpdateUserPasswordFails_ReturnsError()
    {
        var user = new UserLoginDto { PasswordHash = Convert.ToBase64String(new byte[] { 1 }), Salt = Convert.ToBase64String(new byte[] { 2 }), Email = "user@example.com", Id = 1 };
        _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(user);
        _cryptoService.Setup(x => x.HashPasswordWithSalt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(new byte[] { 1 });
        _cryptoService.Setup(x => x.FixedTimeEquals(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(true);
        _systemPoliciesRepository.Setup(x => x.GetDefaultSystemPolicyAsync())
            .ReturnsAsync(new SystemPolicyDto
            {
                MinLength = ValidSystemPolicy.MinLength,
                MaxLength = ValidSystemPolicy.MaxLength,
                NoOfUpperCase = ValidSystemPolicy.NoOfUpperCase,
                NoOfLowerCase = ValidSystemPolicy.NoOfLowerCase,
                NoOfSpecialCharacters = ValidSystemPolicy.NoOfSpecialCharacters,
                SpecialCharactersAllowed = ValidSystemPolicy.SpecialCharactersAllowed,
                HistoricalPasswords = ValidSystemPolicy.HistoricalPasswords
            });
        _userPasswordHistoryRepository.Setup(x => x.GetHistoricalPasswordDataForUserAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<UserPasswordHistoryDataDto>());
        _cryptoService.Setup(x => x.GenerateSalt()).Returns(Convert.ToBase64String(new byte[] { 3 }));
        _usersRepository.Setup(x => x.UpdateUserPasswordAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();
        var result = await handler.Handle(new ChangePasswordCommand(ValidRequestDto), CancellationToken.None);

        Assert.False(result.Status);
        Assert.Equal(AppConstants.ErrorCodes.PASSWORD_CHANGE_FAILED, result.Message);
    }

    [Fact]
    public async Task Handle_CreatePasswordHistoryFails_ReturnsError()
    {
        var user = new UserLoginDto { PasswordHash = Convert.ToBase64String(new byte[] { 1 }), Salt = Convert.ToBase64String(new byte[] { 2 }), Email = "user@example.com", Id = 1 };
        _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(user);
        _cryptoService.Setup(x => x.HashPasswordWithSalt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(new byte[] { 1 });
        _cryptoService.Setup(x => x.FixedTimeEquals(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(true);
        _systemPoliciesRepository.Setup(x => x.GetDefaultSystemPolicyAsync())
           .ReturnsAsync(new SystemPolicyDto
           {
               MinLength = ValidSystemPolicy.MinLength,
               MaxLength = ValidSystemPolicy.MaxLength,
               NoOfUpperCase = ValidSystemPolicy.NoOfUpperCase,
               NoOfLowerCase = ValidSystemPolicy.NoOfLowerCase,
               NoOfSpecialCharacters = ValidSystemPolicy.NoOfSpecialCharacters,
               SpecialCharactersAllowed = ValidSystemPolicy.SpecialCharactersAllowed,
               HistoricalPasswords = ValidSystemPolicy.HistoricalPasswords
           });
        _userPasswordHistoryRepository.Setup(x => x.GetHistoricalPasswordDataForUserAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<UserPasswordHistoryDataDto>());
        _cryptoService.Setup(x => x.GenerateSalt()).Returns(Convert.ToBase64String(new byte[] { 3 }));
        _usersRepository.Setup(x => x.UpdateUserPasswordAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _userPasswordHistoryRepository.Setup(x => x.CreatePasswordHistory(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        var handler = CreateHandler();
        var result = await handler.Handle(new ChangePasswordCommand(ValidRequestDto), CancellationToken.None);

        Assert.False(result.Status);
        Assert.Equal(AppConstants.ErrorCodes.PASSWORD_CHANGE_FAILED, result.Message);
    }

    [Fact]
    public async Task Handle_SuccessfulPasswordChange_ReturnsSuccess()
    {
        var user = new UserLoginDto { PasswordHash = Convert.ToBase64String(new byte[] { 1 }), Salt = Convert.ToBase64String(new byte[] { 2 }), Email = "user@example.com", Id = 1 };
        _usersRepository.Setup(x => x.GetUserByEmailAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(user);
        _cryptoService.Setup(x => x.HashPasswordWithSalt(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(new byte[] { 1 });
        _cryptoService.Setup(x => x.FixedTimeEquals(It.IsAny<byte[]>(), It.IsAny<byte[]>()))
            .Returns(true);
        _systemPoliciesRepository.Setup(x => x.GetDefaultSystemPolicyAsync())
              .ReturnsAsync(new SystemPolicyDto
              {
                  MinLength = ValidSystemPolicy.MinLength,
                  MaxLength = ValidSystemPolicy.MaxLength,
                  NoOfUpperCase = ValidSystemPolicy.NoOfUpperCase,
                  NoOfLowerCase = ValidSystemPolicy.NoOfLowerCase,
                  NoOfSpecialCharacters = ValidSystemPolicy.NoOfSpecialCharacters,
                  SpecialCharactersAllowed = ValidSystemPolicy.SpecialCharactersAllowed,
                  HistoricalPasswords = ValidSystemPolicy.HistoricalPasswords
              });
        _userPasswordHistoryRepository.Setup(x => x.GetHistoricalPasswordDataForUserAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(new List<UserPasswordHistoryDataDto>());
        _cryptoService.Setup(x => x.GenerateSalt()).Returns(Convert.ToBase64String(new byte[] { 3 }));
        _usersRepository.Setup(x => x.UpdateUserPasswordAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _userPasswordHistoryRepository.Setup(x => x.CreatePasswordHistory(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
        _userPasswordHistoryRepository.Setup(x => x.DeleteOldPasswordHistoryAsync(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(true);
        _notificationsRepository.Setup(x => x.InsertNotificationAsync(It.IsAny<NotificationDto>()))
            .ReturnsAsync(true);

        var handler = CreateHandler();
        var result = await handler.Handle(new ChangePasswordCommand(ValidRequestDto), CancellationToken.None);

        Assert.True(result.Status);
        Assert.Equal("Password updated successfully.", result.Message);
    }
}