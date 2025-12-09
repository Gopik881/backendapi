using Elixir.Application.Features.Module.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface ISubModulesRepository
{
    Task<Tuple<List<SubModuleDto>, int>> GetFilteredSubModulesByModuleAsync(int moduleId, string searchTerm, int pageNumber, int pageSize);
}