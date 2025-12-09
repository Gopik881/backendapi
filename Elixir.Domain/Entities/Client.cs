namespace Elixir.Domain.Entities;

public partial class Client
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string ClientName { get; set; } = null!;

    public string? ClientInfo { get; set; }

    public bool? IsEnabled { get; set; }

    public string ClientCode { get; set; } = null!;

    public string? Address1 { get; set; }

    public string? Address2 { get; set; }

    public int? StateId { get; set; }

    public int? CountryId { get; set; }

    public string? ZipCode { get; set; }

    public string? PhoneNumber { get; set; }

    public int? PhoneCodeId { get; set; }

    public bool IsDeleted { get; set; }

}
