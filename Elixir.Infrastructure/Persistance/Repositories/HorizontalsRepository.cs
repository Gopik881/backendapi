using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class HorizontalsRepository : IHorizontalsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public HorizontalsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> AddHorizontalsAsync(int groupId, List<UserGroupHorizontals> horizontals)
    {
        var horizontalEntities = horizontals.Select(h => new Horizontal
        {
            UserGroupId = groupId,
            HorizontalName = h.HorizontalName,
            Description = h.Description,
            IsSelected = h.IsSelected,
        }).ToList();

        await _dbContext.Horizontals.AddRangeAsync(horizontalEntities);
        await _dbContext.SaveChangesAsync();

        var checkboxItems = horizontals
            .SelectMany((h, i) => h.HorizontalItems.Select(item => new WebQueryHorizontalCheckboxItem
            {
                HorizontalId = horizontalEntities[i].Id,
                CheckboxItemName = item.ItemName,
                IsSelected = item.IsSelected
            })).ToList();

        if (checkboxItems.Count > 0)
        {
            await _dbContext.WebQueryHorizontalCheckboxItems.AddRangeAsync(checkboxItems);
            await _dbContext.SaveChangesAsync();
        }

        return true;
    }
    public async Task<bool> UpdateHorizontalsAsync(int groupId, List<UserGroupHorizontals> horizontals)
    {
        // Remove existing horizontals and their checkbox items for the group
        var existingHorizontals = await _dbContext.Horizontals
            .Where(h => h.UserGroupId == groupId)
            .ToListAsync();

        if (existingHorizontals.Any())
        {
            var horizontalIds = existingHorizontals.Select(h => h.Id).ToList();
            var existingCheckboxItems = await _dbContext.WebQueryHorizontalCheckboxItems
                .Where(c => horizontalIds.Contains(c.HorizontalId))
                .ToListAsync();

            if (existingCheckboxItems.Any())
            {
                _dbContext.WebQueryHorizontalCheckboxItems.RemoveRange(existingCheckboxItems);
                await _dbContext.SaveChangesAsync();
            }

            _dbContext.Horizontals.RemoveRange(existingHorizontals);
            await _dbContext.SaveChangesAsync();
        }

        // Add new horizontals and their checkbox items
        var newHorizontals = horizontals.Select(h => new Horizontal
        {
            UserGroupId = groupId,
            HorizontalName = h.HorizontalName,
            Description = h.Description,
            IsSelected = h.IsSelected,
        }).ToList();

        await _dbContext.Horizontals.AddRangeAsync(newHorizontals);
        await _dbContext.SaveChangesAsync();

        var checkboxItems = horizontals
            .SelectMany((h, i) => h.HorizontalItems.Select(item => new WebQueryHorizontalCheckboxItem
            {
                HorizontalId = newHorizontals[i].Id,
                CheckboxItemName = item.ItemName,
                IsSelected = item.IsSelected
            })).ToList();

        if (checkboxItems.Count > 0)
        {
            await _dbContext.WebQueryHorizontalCheckboxItems.AddRangeAsync(checkboxItems);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        return false;
    }
    public async Task<List<UserGroupHorizontals>> GetHorizontalsForRoleAsync(int userGroupId)
    {
        return await _dbContext.Horizontals
            .Where(h => h.UserGroupId == userGroupId)
            .Select(h => new UserGroupHorizontals
            {
                Id = h.Id,
                HorizontalName = h.HorizontalName,
                Description = h.Description,
                IsSelected = h.IsSelected, // Set as needed, since Horizontal does not have IsSelected
                HorizontalItems = _dbContext.WebQueryHorizontalCheckboxItems
                    .Where(ci => ci.HorizontalId == h.Id)
                    .Select(ci => new HorizontalItem
                    {
                        Id = ci.Id,
                        ItemName = ci.CheckboxItemName,
                        IsSelected = ci.IsSelected ?? false
                    }).ToList()
            })
            .ToListAsync();
    }

    public async Task<bool> DeleteHorizontalsByUserGroupIdAsync(int groupId)
    {
        // Get horizontals for the group
        var horizontals = await _dbContext.Horizontals
            .Where(h => h.UserGroupId == groupId)
            .ToListAsync();

        if (horizontals.Count == 0)
            return true;

        // Get related checkbox items
        var horizontalIds = horizontals.Select(h => h.Id).ToList();
        var checkboxItems = await _dbContext.WebQueryHorizontalCheckboxItems
            .Where(ci => horizontalIds.Contains(ci.HorizontalId))
            .ToListAsync();

        // Remove checkbox items first to avoid FK constraint violation
        if (checkboxItems.Count > 0)
        {
            _dbContext.WebQueryHorizontalCheckboxItems.RemoveRange(checkboxItems);
            await _dbContext.SaveChangesAsync();
        }

        _dbContext.Horizontals.RemoveRange(horizontals);

        return await _dbContext.SaveChangesAsync() > 0;
    }

    //public async Task<UserGroup?> CheckForDuplicateHorizontalsAsync(List<UserGroupHorizontals> horizontals, int? userGroupId = 0)
    //{
    //    // Normalize provided horizontals for comparison (only isSelected and names matter)
    //    var providedHorizontals = horizontals
    //        .Select(h => new
    //        {
    //            HorizontalName = h.HorizontalName.Trim(),
    //            IsSelected = h.IsSelected,
    //            CheckboxItems = h.HorizontalItems != null
    //                ? h.HorizontalItems
    //                    .Select(item => new
    //                    {
    //                        CheckboxItemName = (item.ItemName ?? string.Empty).Trim(),
    //                        IsSelected = item.IsSelected
    //                    })
    //                    .OrderBy(x => x.CheckboxItemName)
    //                    .ThenBy(x => x.IsSelected)
    //                    .Cast<object>()
    //                    .ToList()
    //                : new List<object>()
    //        })
    //        .OrderBy(x => x.HorizontalName)
    //        .ThenBy(x => x.IsSelected)
    //        .ToList();

    //    // Query all horizontals except the current user group (if provided)
    //    var groupMappingsQuery = _dbContext.Horizontals.AsQueryable();
    //    if (userGroupId > 0)
    //    {
    //        groupMappingsQuery = groupMappingsQuery.Where(m => m.UserGroupId != userGroupId);
    //    }
    //    var groupMappings = await groupMappingsQuery.ToListAsync();

    //    // Get all checkbox items for these horizontals
    //    var horizontalIds = groupMappings.Select(h => h.Id).ToList();
    //    var allCheckboxItems = await _dbContext.WebQueryHorizontalCheckboxItems
    //        .Where(ci => horizontalIds.Contains(ci.HorizontalId))
    //        .ToListAsync();

    //    // Group horizontals and their checkbox items by UserGroupId
    //    var groupHorizontals = groupMappings
    //        .GroupBy(h => h.UserGroupId)
    //        .Select(g =>
    //        {
    //            var horizontalsList = g.Select(h =>
    //                new
    //                {
    //                    HorizontalName = h.HorizontalName.Trim(),
    //                    IsSelected = h.IsSelected,
    //                    CheckboxItems = allCheckboxItems
    //                        .Where(ci => ci.HorizontalId == h.Id)
    //                        .Select(ci => new
    //                        {
    //                            CheckboxItemName = (ci.CheckboxItemName ?? string.Empty).Trim(),
    //                            IsSelected = ci.IsSelected ?? false
    //                        })
    //                        .OrderBy(x => x.CheckboxItemName)
    //                        .ThenBy(x => x.IsSelected)
    //                        .ToList()
    //                })
    //                .OrderBy(x => x.HorizontalName)
    //                .ThenBy(x => x.IsSelected)
    //                .ToList();

    //            return new
    //            {
    //                GroupId = g.Key,
    //                Horizontals = horizontalsList
    //            };
    //        })
    //        .ToList();

    //    // Fix for CS1061: Cast 'object' to dynamic to access 'CheckboxItemName' and 'IsSelected' properties
    //    var match = groupHorizontals.FirstOrDefault(gr =>
    //        gr.Horizontals.Count == providedHorizontals.Count &&
    //        gr.Horizontals.Zip(providedHorizontals, (a, b) =>
    //            a.HorizontalName == b.HorizontalName &&
    //            a.IsSelected == b.IsSelected &&
    //            // Compare CheckboxItemName only for IsSelected == true
    //            ((a.CheckboxItems
    //                .Where(x => ((dynamic)x).IsSelected)
    //                .Select(x => ((dynamic)x).CheckboxItemName)
    //                .OrderBy(x => x)
    //                .SequenceEqual(
    //                    b.CheckboxItems
    //                        .Where(y => ((dynamic)y).IsSelected)
    //                        .Select(y => ((dynamic)y).CheckboxItemName)
    //                        .OrderBy(y => y)
    //                )
    //            ))
    //        ).All(equal => equal)
    //    );
    //    // Removed dynamic casts; now using strong typing after removing .Cast<object>()
    //    var matches = groupHorizontals.Where(gr =>
    //        gr.Horizontals.Count == providedHorizontals.Count &&
    //        gr.Horizontals.Zip(providedHorizontals, (a, b) =>
    //            a.HorizontalName == b.HorizontalName &&
    //            a.IsSelected == b.IsSelected &&
    //            // Compare CheckboxItemName only for IsSelected == true
    //            a.CheckboxItems
    //                .Where(x => x.IsSelected)
    //                .Select(x => x.CheckboxItemName)
    //                .OrderBy(x => x)
    //                .SequenceEqual(
    //                    b.CheckboxItems
    //                        .Where(y => ((dynamic)y).IsSelected)
    //                        .Select(y => ((dynamic)y).CheckboxItemName)
    //                        .OrderBy(y => y)
    //                )
    //        ).All(equal => equal)
    //    ).ToList();

    //    return match != null
    //        ? await _dbContext.UserGroups.FirstOrDefaultAsync(u => u.Id == match.GroupId)
    //        : null;
    //}

    // Modified Section 2
    public async Task<List<UserGroup>> CheckForDuplicateHorizontalsAsync(List<UserGroupHorizontals> horizontals, int? userGroupId = 0)
    {
        // Normalize provided horizontals for comparison (only isSelected and names matter)
        // Normalize provided horizontals for comparison (only isSelected and names matter)
        var providedHorizontals = horizontals
            .Select(h => new
            {
                HorizontalName = h.HorizontalName.Trim(),
                IsSelected = h.IsSelected,
                CheckboxItems = h.HorizontalItems != null
                    ? h.HorizontalItems
                        .Select(item => new
                        {
                            CheckboxItemName = (item.ItemName ?? string.Empty).Trim(),
                            IsSelected = item.IsSelected
                        })
                        .OrderBy(x => x.CheckboxItemName)
                        .ThenBy(x => x.IsSelected)
                        .Cast<object>()
                        .ToList()
                    : new List<object>()
            })
            .OrderBy(x => x.HorizontalName)
            .ThenBy(x => x.IsSelected)
            .ToList();

        // Query all horizontals except the current user group (if provided)
        var groupMappingsQuery = _dbContext.Horizontals.AsQueryable();
        if (userGroupId > 0)
        {
            groupMappingsQuery = groupMappingsQuery.Where(m => m.UserGroupId != userGroupId);
        }
        var groupMappings = await groupMappingsQuery.ToListAsync();

        // Get all checkbox items for these horizontals
        var horizontalIds = groupMappings.Select(h => h.Id).ToList(); // Fixed: Use h.Id instead of UserGroupId for checkbox query
        var allCheckboxItems = await _dbContext.WebQueryHorizontalCheckboxItems
            .Where(ci => horizontalIds.Contains(ci.HorizontalId))
            .ToListAsync();

        // Group horizontals and their checkbox items by UserGroupId
        var groupHorizontals = groupMappings
            .GroupBy(h => h.UserGroupId)
            .Select(g =>
            {
                var horizontalsList = g.Select(h =>
                    new
                    {
                        HorizontalName = h.HorizontalName.Trim(),
                        IsSelected = h.IsSelected,
                        CheckboxItems = allCheckboxItems
                            .Where(ci => ci.HorizontalId == h.Id)
                            .Select(ci => new
                            {
                                CheckboxItemName = (ci.CheckboxItemName ?? string.Empty).Trim(),
                                IsSelected = ci.IsSelected ?? false
                            })
                            .OrderBy(x => x.CheckboxItemName)
                            .ThenBy(x => x.IsSelected)
                            .ToList()
                    })
                    .OrderBy(x => x.HorizontalName)
                    .ThenBy(x => x.IsSelected)
                    .ToList();
                return new
                {
                    GroupId = g.Key,
                    Horizontals = horizontalsList
                };
            })
            .ToList();

        // Removed dynamic casts; now using strong typing after removing .Cast<object>()
        var matches = groupHorizontals.Where(gr =>
            gr.Horizontals.Count == providedHorizontals.Count &&
            gr.Horizontals.Zip(providedHorizontals, (a, b) =>
                a.HorizontalName == b.HorizontalName &&
                a.IsSelected == b.IsSelected &&
                // Compare CheckboxItemName only for IsSelected == true
                a.CheckboxItems
                    .Where(x => x.IsSelected)
                    .Select(x => x.CheckboxItemName)
                    .OrderBy(x => x)
                    .SequenceEqual(
                        b.CheckboxItems
                            .Where(y => ((dynamic)y).IsSelected)
                            .Select(y => ((dynamic)y).CheckboxItemName)
                            .OrderBy(y => y)
                    )
            ).All(equal => equal)
        ).ToList();

        var matchingGroupIds = matches.Select(m => m.GroupId).ToList();

        return await _dbContext.UserGroups
            .Where(u => matchingGroupIds.Contains(u.Id))
            .ToListAsync();
    }
}
