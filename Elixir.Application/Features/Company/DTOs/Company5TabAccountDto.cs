namespace Elixir.Application.Features.Company.DTOs;

public class Company5TabAccountDto
{
    public int? CompanyStorageGB { get; set; }
    public int? PerUserStorageMB { get; set; }
    public int? UserGroupLimit { get; set; }
    public int? TempUserLimit { get; set; }
    public string? ContractName { get; set; }
    public string? ContractId { get; set; }
    public DateTime? StartDate { get; set; }
    public bool? OpenEnded { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? RenewalReminderDate { get; set; }
    public int? ContractNoticePeriod { get; set; }
    public string? LicenseProcurement { get; set; }
    public string? Pan { get; set; }
    public string? Tan { get; set; }
    public string? Gstn { get; set; }
}
