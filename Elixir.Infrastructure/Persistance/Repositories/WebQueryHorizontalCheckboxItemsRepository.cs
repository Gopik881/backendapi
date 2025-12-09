using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class WebQueryHorizontalCheckboxItemsRepository : IWebQueryHorizontalCheckboxItemsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public WebQueryHorizontalCheckboxItemsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}
