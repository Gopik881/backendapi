using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class WebQueryAdminRepository : IWebQueryAdminRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public WebQueryAdminRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
