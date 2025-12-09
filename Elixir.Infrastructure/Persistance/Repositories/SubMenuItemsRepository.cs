using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;


namespace Elixir.Infrastructure.Persistance.Repositories;

public class SubMenuItemsRepository : ISubMenuItemsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public SubMenuItemsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
