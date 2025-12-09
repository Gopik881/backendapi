namespace Elixir.Application.Features.Company.DTOs;

public class CompanySummaryDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientCode { get; set; } = string.Empty;
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public decimal CompanyStorageConsumedGB { get; set; }
    public decimal CompanyStorageTotalGB { get; set; }
    public decimal PerUserStorageMB { get; set; }
    public string? DisplaydCompanyStorageConsumedGB { get; set; }
    public string? DisplaydPerUserStorageMB { get; set; }
    public int Users { get; set; }
    public int UserGroups { get; set; }
    public bool IsEnabled { get; set; }
    public bool IsUnderEdit { get; set; }

    public int? ClientCompaniesCount { get; set; }
    public int? ClientAccountManagersCount { get; set; }
    public string? UserName { get; set; }
    
    public string? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; }
    public string? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }

    public bool? CompanyStatus { get; set; } = null; // Nullable to allow for no status

}
