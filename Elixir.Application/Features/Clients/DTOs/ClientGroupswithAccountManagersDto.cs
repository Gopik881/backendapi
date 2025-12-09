using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.User.DTOs;

namespace Elixir.Application.Features.Clients.DTOs;

public class ClientGroupswithAccountManagersDto
{
    public List<UserGroupDto> clientUserGroups { get; set; } = new List<UserGroupDto>();
    public List<CompanyUserDto> clientAccountManagers { get; set; }
}
