namespace Elixir.Domain.Entities;

public partial class EscalationContact
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int CompanyId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? LastName { get; set; }

    public int? TelephoneCodeId { get; set; }

    public string? PhoneNumber { get; set; }

    public string Email { get; set; } = null!;

    public string? Designation { get; set; }

    public string? Department { get; set; }

    public string? Remarks { get; set; }

    public bool IsDeleted { get; set; }

}
