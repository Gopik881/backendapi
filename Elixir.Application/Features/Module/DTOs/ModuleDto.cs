namespace Elixir.Application.Features.Module.DTOs;
public class ModuleDto
{
    public int? ModuleId { get; set; }
    public string ModuleName { get; set; }
    public List<SubModuleDto> SubModules { get; set; } = null;
}
