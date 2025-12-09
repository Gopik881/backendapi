using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Features.UserGroup.DTOs;

public class UserMappedGroupNamesWithCompaniesDto
{
    public List<UserGroupNamesWithCount> userGroupNamesWithCounts { get; set; }
    public UserProfileDto userDetails { get; set; }
    public List<UserMappedCompaniesDto>? userMappedCompanies { get; set; }

    public List<UserMappedClientsDto>? clientsMappedUsers { get; set; }
}

public class UserGroupNamesWithCount
{
    public string GroupType { get; set; }
    public List<string> GroupNames { get; set; }
    public int? companyCount { get; set; }


}

public class UserMappedCompaniesDto
{
    public int? groupId { get; set; }
    public string? GroupName { get; set; }
    public List<string>? CompanyNames { get; set; }
    public int? CompanyCount { get; set; }
}

public class UserMappedClientsDto
{
    public int? groupId { get; set; }
    public string? GroupName { get; set; }
    public List<string>? ClientNames { get; set; }
    public int? ClientCount { get; set; }
}