using Elixir.Application.Interfaces.Persistance;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;


namespace Elixir.Infrastructure.Persistance.Repositories
{
   public class TransactionRepository : ITransactionRepository
    {
        private readonly ElixirHRDbContext _dbContext;
        private IDbContextTransaction _dbContextTransaction;

        public TransactionRepository(ElixirHRDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task BeginTransactionAsync()
        {
            _dbContextTransaction = await _dbContext.Database.BeginTransactionAsync();
        }
        public async Task CommitAsync()
        {
            if(_dbContextTransaction != null)
            {
                await _dbContextTransaction.CommitAsync();
            }
        }
        public void Dispose()
        {
            _dbContextTransaction.Dispose();
        }
        public async Task RollbackAsync()
        {
            await _dbContextTransaction.RollbackAsync();
        }
    }
}
