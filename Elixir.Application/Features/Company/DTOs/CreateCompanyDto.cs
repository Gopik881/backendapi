using Elixir.Application.Features.User.DTOs;

namespace Elixir.Application.Features.Company.DTOs;

public class CreateCompanyDto
{
    public string CompanyName { get; set; }
    public List<ElixirUserCreateCompanyDto> elixirUsers { get; set; }
    public bool? CompanyStatus { get; set; }
    public string? OnboardingStatus { get; set; }
    public int? GroupsCount { get; set; }
    public bool? isUnderEdit { get; set; }
}




public class ElixirUserCreateCompanyDto
{
    public int RoleId { get; set; }
    public int UserGroupId { get; set; }
    public int? UserId { get; set; }
}