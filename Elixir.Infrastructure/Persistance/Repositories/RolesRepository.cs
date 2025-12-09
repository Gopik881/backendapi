using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class RolesRepository : IRolesRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public RolesRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

}
