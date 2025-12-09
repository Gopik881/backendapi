using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class SubMenuItemsAccessMappingRepository : ISubMenuItemsAccessMappingRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public SubMenuItemsAccessMappingRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

}
