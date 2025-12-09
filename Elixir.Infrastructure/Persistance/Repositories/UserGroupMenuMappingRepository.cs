using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class UserGroupMenuMappingRepository : IUserGroupMenuMappingRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public UserGroupMenuMappingRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> DeleteUserRightsByUserGroupIdAsync(int groupId)
    {
        // Remove UserGroupMenuMappings
        var userRights = _dbContext.UserGroupMenuMappings.Where(sp => sp.UserGroupId == groupId).ToList();
        _dbContext.UserGroupMenuMappings.RemoveRange(userRights);

        // Remove dependent records in ElixirUsersHistory
        var dependentHistory = _dbContext.ElixirUsersHistories.Where(euh => euh.UserGroupId == groupId).ToList();
        if (dependentHistory.Any())
        {
            _dbContext.ElixirUsersHistories.RemoveRange(dependentHistory);
        }

        // Remove dependent records in ElixirUsers
        var dependentUsers = _dbContext.ElixirUsers.Where(eu => eu.UserGroupId == groupId).ToList();
        if (dependentUsers.Any())
        {
            _dbContext.ElixirUsers.RemoveRange(dependentUsers);
        }

        // Remove the UserGroup itself
        var userGroup = await _dbContext.UserGroups.FindAsync(groupId);
        if (userGroup != null)
        {
            _dbContext.UserGroups.Remove(userGroup);
        }

        return await _dbContext.SaveChangesAsync() > 0;
    }
}
