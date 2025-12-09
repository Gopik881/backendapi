namespace Elixir.Application.Features.Company.DTOs;

public class SuperAdminDashBoardDetailsDto
{
    public int ActiveCompaniesCount { get; set; }
    public int OnboardingCompaniesCount { get; set; }
    public int ElixirUserGroupsCount { get; set; }
    public int ElixirUsersCount { get; set; }
    public string CompanyStorageInGB { get; set; }
    public int ClientsCount { get; set; }
    public int? OnboardingClientsCount { get; set; }
    public string UserStorageInGB { get; set; }
}
