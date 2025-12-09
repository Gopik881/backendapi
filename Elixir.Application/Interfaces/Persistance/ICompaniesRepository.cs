using Elixir.Application.Features.Clients.DTOs;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Domain.Entities;
using MediatR;

namespace Elixir.Application.Interfaces.Persistance;

public interface ICompaniesRepository
{
    Task<bool> ExistsWithCompanyCodeAsync(string companyCode);

    Task<int> FindCompanyByCodeAsync(string companyCode);
    Task<bool> ExistsWithCompanyNameAsync(string companyName);
    Task<bool> ExistsWithCompanyNameForUpdateAsync(string companyName, int companyId);
    Task<Tuple<List<CompanyUserDto>, int>> GetFilteredCompanyUsersAsync(int CompanyId, string searchTerm, int pageNumber, int pageSize);
    Task<Tuple<List<CompanyBasicInfoDto>, int>> GetFilteredCompanyByUsersAsync(int userId, int groupId, string groupName, string searchTerm, int pageNumber, int pageSize);
    Task<bool> UpdateCompanyUnderEditAsync(int companyId, int userId, bool isUnderEdit);
    Task<CompanyBasicInfoDto> GetCompanyBasicInfoAsync(int companyId);
    Task<Tuple<List<CompanyTMISummaryDto>, int>> GetPagedTMIUsersCompaniesSummaryAsync(int userId, bool IsUnderEdit, string searchTerm, int pageNumber, int pageSize);
    Task<Tuple<List<CompanySummaryDto>, int>> GetPagedSuperAdminCompaniesSummaryAsync(int userId, bool IsUnderEdit, bool IsSuperUser, string searchTerm, int pageNumber, int pageSize);
    Task<Tuple<List<CompanySummaryDto>, int>> GetPagedDelegateAdminCompaniesSummaryAsync(int userId, bool IsUnderEdit,  bool IsSuperAdmin, string searchTerm, int pageNumber, int pageSize);
    Task<bool> Company5TabApproveCompanyDataAsync(int userId, Company5TabCompanyDto company5TabCompanyData, int companyStorageGB, int perUserStorageGB, CancellationToken cancellationToken = default);
    Task<bool> AddClientAccountManagersAsync(CreateClientDto createClientDto, int userId, int clientId);
    Task<Company> GetCompanyByIdAsync(int companyId);
    Task UpdateCompanyAsync(Company company);
    Task<int> CreateCompanyAsync(int userId, CreateCompanyDto companyDto, CancellationToken cancellationToken = default);
    Task<bool> UpdateCompanyAsync(int companyId, int userId, CreateCompanyDto companyDto, CancellationToken cancellationToken = default);
    Task<IEnumerable<object>> GetCompanyPopupDetailsByCompanyIdAsync(int companyId);
    Task<SuperAdminDashBoardDetailsDto> GetSuperAdminDashBoardDetailsAsync();
    Task<TmiDashBoardDetailsDto> GetTMIAdminDashBoardDetailsAsync(int userId);
    //Task<List<UserGroupDto>> GetCompany5TabCustomUserGroups(int companyId);
    Task<List<UserGroupDto>> GetCompany5TabCustomUserGroups(int companyId, string? ScreenName = "");

    Task CloneElixirTenantDatabaseAsync(string sourceDb, string targetDb, string elasticPool);

    Task<bool> UpdateCompanyLastUpdatedBy(int companyId, int userId);
    Task<bool> UpdateCompanyHistoryLastUpdatedBy(int companyId, int userId);
}