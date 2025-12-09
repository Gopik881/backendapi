namespace Elixir.Domain.Entities;

public partial class SuperUser
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int EmailHash { get; set; }

    public string? PasswordHash { get; set; }

    public string? Salt { get; set; }

    public int? TelephoneCodeId { get; set; }
    
    public string PhoneNumber { get; set; } = null!;

    public string? Location { get; set; }

    public string Designation { get; set; } = null!;

    public string? ProfilePicture { get; set; }

    public bool? IsEnabled { get; set; }
    public DateTime? LastFailedAttempt { get; set; }
    public int? FailedLoginAttempts { get; set; }
    public bool IsLockedOut { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? LastSessionActiveUntil { get; set; }
    public string? ResetPasswordToken { get; set; }

}
