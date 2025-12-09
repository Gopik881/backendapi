using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;


namespace Elixir.Infrastructure.Persistance.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public CategoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
