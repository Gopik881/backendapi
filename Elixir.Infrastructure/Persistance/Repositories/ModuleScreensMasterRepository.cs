using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ModuleScreensMasterRepository : IModuleScreensMasterRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ModuleScreensMasterRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

}
