namespace Elixir.Application.Features.Company.DTOs;

public class CompanyBasicInfoDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string CompanyCode { get; set; } = string.Empty;
    public DateTime? CreatedOn { get; set; }

}
