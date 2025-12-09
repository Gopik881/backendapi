using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Infrastructure.Persistance.Repositories
{
    public class BulkUploadErrorListRepository : IBulkUploadErrorListRepository
    {
        private readonly ElixirHRDbContext _dbContext;
        public BulkUploadErrorListRepository(ElixirHRDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<bool> BulkInsertBulkUploadErrorListAsync(List<BulkUploadErrorList> errors)
        {
            await _dbContext.BulkUploadErrorLists.AddRangeAsync(errors);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Tuple<List<BulkUploadErrorListDto>, int>> GetPagedBulkUploadErrorListAsync(Guid ProcessId, int pageNumber, int pageSize)
        {
            var query = _dbContext.BulkUploadErrorLists.Where(b => b.ProcessId == ProcessId).OrderBy(b => b.Id);
            var totalCount = await query.CountAsync();
            var errors = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize)
                .Select(u => new BulkUploadErrorListDto
                {
                    S_No = u.RowId,
                    Field = u.ErrorField,
                    Error = u.ErrorMessage
                }).ToListAsync();
            return new Tuple<List<BulkUploadErrorListDto>, int>(errors, totalCount);
        }


        public async Task<List<BulkUploadErrorListDto>> GetBulkUploadErrorListAsync(Guid ProcessId)
        {
            var query = _dbContext.BulkUploadErrorLists.Where(b => b.ProcessId == ProcessId).OrderBy(b => b.Id);
            return await query.Select(u => new BulkUploadErrorListDto
                {
                    S_No = u.RowId,
                    Field = u.ErrorField,
                    Error = u.ErrorMessage
                }).ToListAsync();
        }

        public async Task<bool> DeleteBulkUploadErrorListAsync(Guid ProcessId)
        {
            var query = _dbContext.BulkUploadErrorLists.Where(b => b.ProcessId == ProcessId).OrderBy(b => b.Id);
            _dbContext.BulkUploadErrorLists.RemoveRange(query);
            await _dbContext.SaveChangesAsync();
            return true;

        }

    }
}
