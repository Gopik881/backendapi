using Elixir.Application.Features.Master.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IMasterRepository
{
    Task<Tuple<List<MasterDto>, int>> GetFilteredMasterAsync(string searchTerm, int pageNumber, int pageSize);
}