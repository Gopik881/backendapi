using Elixir.Application.Features.Company.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface ICompanyAdminUsersRepository
{
    Task<bool> Company5TabApproveCompanyAdminDataAsync(int companyId, int userId, Company5TabCompanyAdminDto company5TabCompanyAdmin, CancellationToken cancellationToken = default);
    Task<CompanyAdminUserDto> GetCompanyAdminUserByEmailAsync(string email, int emailHash);

    Task<bool> UpdateExistsCompanyAdminByEmailAsync(string email, int companyId);
    Task<bool> ExistsCompanyAdminByEmailAsync(string email);
}