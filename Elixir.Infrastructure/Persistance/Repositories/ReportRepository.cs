using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ReportRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

}
