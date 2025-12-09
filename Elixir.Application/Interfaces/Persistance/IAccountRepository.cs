using Elixir.Application.Features.Company.DTOs;
using Elixir.Domain.Entities;

namespace Elixir.Application.Interfaces.Persistance;

public  interface IAccountRepository
{
    Task CreateAsync(Account account);
    Task DeleteAsync(int id);
    Task UpdateAsync(int id, Account updatedAccount);
    Task<bool> Company5TabApproveAccountInfoAsync(int companyId, int userId, Company5TabAccountDto company5TabAccount, CancellationToken cancellationToken = default);
}