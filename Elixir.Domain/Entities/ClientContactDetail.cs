namespace Elixir.Domain.Entities;

public partial class ClientContactDetail
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public int? CountryId { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Designation { get; set; }

    public string? Department { get; set; }

    public string? Remarks { get; set; }

    public int? ClientId { get; set; }

    public bool IsDeleted { get; set; }

}
