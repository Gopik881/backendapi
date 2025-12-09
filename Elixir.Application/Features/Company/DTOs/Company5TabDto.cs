namespace Elixir.Application.Features.Company.DTOs;

public class Company5TabDto
{
   public Company5TabCompanyDto Company5TabCompanyDto { get; set; }
   public Company5TabAccountDto Company5TabAccountDto { get; set; }
   public Company5TabCompanyAdminDto Company5TabCompanyAdminDto { get; set; }
   public List<Company5TabModuleMappingDto> Company5TabModuleMappingDto { get;set; }
   public Company5TabReportingToolLimitsDto Company5TabReportingToolLimitsDto { get; set; }
   public List<Company5TabEscalationContactDto> Company5TabEscalationContactDto { get; set; }
   public List<Company5TabElixirUserDto> company5TabElixirUserDto { get; set; }

    public bool isApproved { get; set; } = false;
    public string RejectedReason { get; set; } = String.Empty;
}
