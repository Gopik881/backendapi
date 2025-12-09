using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Master.DTOs;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class MasterRepository : IMasterRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public MasterRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Tuple<List<MasterDto>, int>> GetFilteredMasterAsync(string searchTerm, int pageNumber, int pageSize)
    {
        //single table so projection and filter applied directly
        var query = _dbContext.Masters
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.Id);

        // Get the count before selecting specific columns and after applying search on DTO columns
        var projectedQuery = query.Select(c => new MasterDto
        {
            MasterId = c.Id,
            MasterName = c.MasterName,
            MasterUrl = c.MasterScreenUrl,
            CreatedOn = c.CreatedAt
        });

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            projectedQuery = projectedQuery.Where(dto =>
                (dto.MasterName != null && dto.MasterName.Contains(searchTerm)) ||
                (dto.MasterUrl != null && dto.MasterUrl.Contains(searchTerm))
                //(dto.CreatedOn != null && dto.CreatedOn.ToString().Contains(searchTerm)) ||
                //dto.MasterId.ToString().Contains(searchTerm)
            );
        }

        var totalCount = await projectedQuery.CountAsync();
        var master = await projectedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new Tuple<List<MasterDto>, int>(master, totalCount);
    }
}
