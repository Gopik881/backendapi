namespace Elixir.Domain.Entities;

public partial class User
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

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

    public decimal? UserStorageConsumed { get; set; }

    public int? FailedLoginAttempts { get; set; }

    public DateTime? LastFailedAttempt { get; set; }

    public DateTime? LastSessionActiveUntil { get; set; }

    public bool IsLockedOut { get; set; }

    public bool IsDeleted { get; set; }

    public string? ResetPasswordToken { get; set; }

}
