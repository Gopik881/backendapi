using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Interfaces.Persistance
{
    public interface IBulkUploadErrorListRepository
    {
        Task<bool> BulkInsertBulkUploadErrorListAsync(List<BulkUploadErrorList> errors);
        Task<Tuple<List<BulkUploadErrorListDto>, int>> GetPagedBulkUploadErrorListAsync(Guid ProcessId, int pageNumber, int pageSize);
        Task<List<BulkUploadErrorListDto>> GetBulkUploadErrorListAsync(Guid ProcessId);
        Task<bool> DeleteBulkUploadErrorListAsync(Guid ProcessId);
    }

}
