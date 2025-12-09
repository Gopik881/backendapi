namespace Elixir.Application.Features.Module.DTOs;

public class ModuleStrucureResponseDto
{
    public int? moduleId { get; set; }
    public string ModuleName { get; set; }
    public bool? IsManadatory { get; set; }
    public List<ModuleDto>? Modules { get; set; }
}


