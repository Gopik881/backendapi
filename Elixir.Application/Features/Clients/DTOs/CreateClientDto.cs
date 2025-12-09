namespace Elixir.Application.Features.Clients.DTOs;

public class CreateClientDto
{
    public ClientInfoDto ClientInfo { get; set; }
    public ClientAdminInfoDto ClientAdminInfo { get; set; }
    public List<ClientContactInfoDto> ClientContactInfo { get; set; }
    public List<ClientAccountManagersDto> ClientAccountManagers { get; set; }
    public ClientAccessDto ClientAccess { get; set; }
    public ReportingToolLimitsDto ReportingToolLimits { get; set; }
    public List<ClientCompanyMappingDto> clientCompanyMappingDtos { get; set; }
}
