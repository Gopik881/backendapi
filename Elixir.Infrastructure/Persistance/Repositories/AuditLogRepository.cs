using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;


namespace Elixir.Infrastructure.Persistance.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public AuditLogRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
