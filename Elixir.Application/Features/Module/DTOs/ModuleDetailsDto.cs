namespace Elixir.Application.Features.Module.DTOs;

public class ModuleDetailsDto
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; }
    public string Description { get; set; }
    public string ModuleURL { get; set; }
    public bool? Status { get; set; }
    public int NoOfCompanies { get; set; }
    public int NoOfSubmodules { get; set; }
    public List<ModuleMasterDto> ModuleMasters { get; set; }
    public List<ModuleScreenDto> ModuleScreens { get; set; }
    public List<SubModuleDtoV2> Submodules { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int UsersCount { get; set; }
}
