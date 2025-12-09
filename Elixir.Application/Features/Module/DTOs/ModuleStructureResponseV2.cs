namespace Elixir.Application.Features.Module.DTOs;

public class ModuleStructureResponseV2
{
    public int? ModuleId { get; set; }
    public string ModuleName { get; set; }
    public List<ModuleMasterDto> ModuleMasters { get; set; }
    public List<ModuleScreenDto> ModuleScreens { get; set; }
    public List<SubModuleDtoV2> Submodules { get; set; }
}

public class ModuleMasterDto
{
    public int MasterId { get; set; }
    public string MasterName { get; set; }
}

public class ModuleScreenDto
{
    public int ModuleScreenId { get; set; }
    public string ScreenName { get; set; }
}

public class SubModuleScreenDto
{
    public string SubModuleName { get; set; }
    public List<MasterScreenDto> Masters { get; set; }
}

public class MasterScreenDto
{
    public string MasterName { get; set; }
    public List<string> Screens { get; set; }
}

public class SubModuleDtoV2
{
    public string SubSet { get; set; }
    public List<SubModuleScreenDto> SubModuleScreens { get; set; }
}
