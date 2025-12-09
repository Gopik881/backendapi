using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Module.DTOs;

namespace Elixir.Application.Interfaces.Persistance;

public interface IModuleMappingRepository
{
    Task<Tuple<List<ModuleUsageDto>, int>> GetFilteredModulesUsageAsync(string searchTerm, int pageNumber, int pageSize);
    Task<Tuple<List<CompanyModuleDto>, int>> GetFilteredCompanyModulesAsync(string FilterBy, int filterId, string searchTerm, int pageNumber, int pageSize);
    Task<bool> Company5TabApproveModuleMappingDataAsync(int companyId, List<Company5TabModuleMappingDto> moduleMappings, int userId, CancellationToken cancellationToken = default);
}