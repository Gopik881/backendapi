using Elixir.Application.Features.Module.DTOs;

namespace Elixir.Application.Interfaces.Persistance;
public interface IModulesRepository
{
    Task<IEnumerable<ModuleDto>> GetAllModulesAsync();
    Task<List<ModuleStrucureResponseDto>> GetModulesWithSubModulesAsync();
    Task<ModuleStructureResponseV2> UpdateModuleStructure(ModuleCreateDto moduleCreation);
    Task<ModuleDetailsDto> GetModuleViewAsync(int moduleId);
    Task<List<object>> GetModuleSubmoduleListAsync(List<int> moduleIds);
    public List<object> GetModuleMastersAndScreens(List<int> moduleIds, bool IsMaster);
}