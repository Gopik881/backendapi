namespace Elixir.Application.Features.User.DTOs;

public class UserLoginDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;
    public string? Salt { get; set; }

    public string? PasswordHash { get; set; }

    public string Email { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public DateTime? LastSessionActiveUntil { get; set; }
    public string? ResetPasswordToken { get; set; }

    public int? FailedLoginAttempts { get; set; }
    public DateTime? LastFailedAttempt { get; set; }

    public bool? isSuperUser { get; set; }
}
