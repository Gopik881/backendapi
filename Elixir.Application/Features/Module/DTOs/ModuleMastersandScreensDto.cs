namespace Elixir.Application.Features.Module.DTOs;

public class ModuleResponseDto
{
    public string ParentName { get; set; }
    public List<ModuleChildrenDto> Children { get; set; }
    public Dictionary<string, List<string>> coreHrChildren { get; set; } = new Dictionary<string, List<string>>();
}

public class ModuleChildrenDto
{
    public string SectionName { get; set; }
    public Dictionary<string, List<string>> Details { get; set; }
}

public class DefaultModuleResponseDto
{
    public string ParentName { get; set; }
    public Dictionary<string, List<string>> Children { get; set; }
}
