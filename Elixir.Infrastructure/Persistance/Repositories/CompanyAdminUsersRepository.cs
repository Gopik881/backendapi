using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class CompanyAdminUsersRepository : ICompanyAdminUsersRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public CompanyAdminUsersRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Company5TabApproveCompanyAdminDataAsync(int companyId, int userId, Company5TabCompanyAdminDto company5TabCompanyAdmin, CancellationToken cancellationToken = default)
    {

        _dbContext.CompanyAdminUsers.Add(new CompanyAdminUser
        {
            CompanyId = companyId,
            FirstName = company5TabCompanyAdmin.CompanyAdminFirstName,
            LastName = company5TabCompanyAdmin.CompanyAdminLastName,
            Email = company5TabCompanyAdmin.CompanyAdminEmailId,
            EmailHash = (int)company5TabCompanyAdmin.CompanyAdminEmailHash,
            TelephoneCodeId = company5TabCompanyAdmin.TelephoneCodeId,
            PhoneNumber = company5TabCompanyAdmin.CompanyAdminPhoneNo,
            Designation = company5TabCompanyAdmin.CompanyAdminDesignation,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            IsEnabled = true,
            UpdatedAt = DateTime.UtcNow,
        });

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<CompanyAdminUserDto> GetCompanyAdminUserByEmailAsync(string email, int emailHash)
    {
        return await _dbContext.CompanyAdminUsers.Where(x => x.EmailHash == emailHash && x.Email == email && !x.IsDeleted && (x.IsEnabled ?? false))
            .Select(u => new CompanyAdminUserDto
            {
                Email = email,
                FirstName = u.FirstName,
                LastName = u.LastName
            }).FirstOrDefaultAsync();
    }


    public Task<bool> UpdateExistsCompanyAdminByEmailAsync(string email, int companyId)
    {
        return _dbContext.CompanyAdminUsersHistories.AnyAsync(x => x.Email == email && x.CompanyId != companyId && !x.IsDeleted && (x.IsEnabled ?? false));
    }

    public Task<bool> ExistsCompanyAdminByEmailAsync(string email)
    {
        return _dbContext.CompanyAdminUsersHistories.AnyAsync(x => x.Email == email && !x.IsDeleted && (x.IsEnabled ?? false));
    }
}
