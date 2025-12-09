namespace Elixir.Domain.Entities;

public partial class CompanyAdminUser
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int? CompanyId { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;
    public int EmailHash { get; set; }

    public int? TelephoneCodeId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Designation { get; set; }

    public bool? IsEnabled { get; set; }

    public string? PasswordHash { get; set; }
    public string? Salt { get; set; }

    public bool IsDeleted { get; set; }

}
