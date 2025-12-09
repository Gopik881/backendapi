using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class UserNotificationsMappingRepository : IUserNotificationsMappingRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public UserNotificationsMappingRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

}
