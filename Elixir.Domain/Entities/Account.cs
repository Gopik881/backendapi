namespace Elixir.Domain.Entities;

public partial class Account
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int CompanyId { get; set; }

    public decimal? PerUserStorageMb { get; set; }

    public int? UserGroupLimit { get; set; }

    public int? TempUserLimit { get; set; }

    public string? ContractName { get; set; }

    public string? ContractId { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public bool? IsOpenEnded { get; set; }

    public DateTime? RenewalReminderDate { get; set; }

    public int? ContractNoticePeriod { get; set; }

    public string? LicenseProcurement { get; set; }

    public string? Pan { get; set; }

    public string? Tan { get; set; }

    public string? Gstin { get; set; }

    public bool IsDeleted { get; set; }
    
}
