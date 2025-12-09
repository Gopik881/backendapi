using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Elixir.Infrastructure.Persistance.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ElixirHRDbContext _dbContext;

        public AccountRepository(ElixirHRDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task CreateAsync(Account account)
        {
            if (account == null) throw new ArgumentNullException(nameof(account));
            await _dbContext.Accounts.AddAsync(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(int id, Account updatedAccount)
        {
            if (updatedAccount == null) throw new ArgumentNullException(nameof(updatedAccount));

            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account == null) throw new KeyNotFoundException("Account not found.");

            account.CompanyId = updatedAccount.CompanyId;
            account.ContractName = updatedAccount.ContractName;

            _dbContext.Accounts.Update(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id);
            if (account == null) throw new KeyNotFoundException("Account not found.");

            _dbContext.Accounts.Remove(account);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> Company5TabApproveAccountInfoAsync(int companyId, int userId, Company5TabAccountDto company5TabAccount, CancellationToken cancellationToken = default)
        {
            // Check if the company exists before proceeding
            var companyExists = await _dbContext.Companies.AnyAsync(c => c.Id == companyId, cancellationToken);
            if (!companyExists)
            {
                throw new KeyNotFoundException($"Company with Id {companyId} does not exist.");
            }

            var accountInfo = await _dbContext.Accounts.FirstOrDefaultAsync(af => af.CompanyId == companyId, cancellationToken)
                ?? _dbContext.Accounts.Add(new Account { CompanyId = companyId, CreatedAt = DateTime.UtcNow }).Entity;

            _dbContext.AccountHistories.Add(new AccountHistory
            {
                CompanyId = companyId,
                PerUserStorageMb = company5TabAccount.PerUserStorageMB,
                UserGroupLimit = company5TabAccount.UserGroupLimit,
                TempUserLimit = company5TabAccount.TempUserLimit,
                ContractName = company5TabAccount.ContractName,
                ContractId = company5TabAccount.ContractId,
                StartDate = company5TabAccount.StartDate,
                EndDate = company5TabAccount.EndDate,
                IsOpenEnded = company5TabAccount.OpenEnded,
                RenewalReminderDate = company5TabAccount.RenewalReminderDate,
                ContractNoticePeriod = company5TabAccount.ContractNoticePeriod,
                LicenseProcurement = company5TabAccount.LicenseProcurement,
                Pan = company5TabAccount.Pan,
                Tan = company5TabAccount.Tan,
                Gstin = company5TabAccount.Gstn,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = userId
            });

            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

       
    }

}
