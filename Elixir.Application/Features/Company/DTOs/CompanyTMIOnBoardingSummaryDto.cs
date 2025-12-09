
namespace Elixir.Application.Features.Company.DTOs;

public class CompanyTMIOnBoardingSummaryDto
{
    public int ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public int CompanyID { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string ClientCode { get; set; }
    public string UsersGroups { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string OnBoardingStatus { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public string? LastUpdatedBy { get; set; }
    public DateTime? LastUpdatedOn { get; set; }

    public int? ClientCompaniesCount { get; set; }
    public int? ClientAccountManagersCount { get; set; }
    public bool? CompanyStatus { get; set; }
    public bool? IsActive { get; set; }

}
