namespace Elixir.Application.Features.Company.DTOs;

public class Company5TabModuleMappingDto
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; }
   
    public List<Company5TabSubModulesDto>? SubModules { get; set; }
}
public class Company5TabSubModulesDto
{
    public int SubModuleId { get; set; }
    public string SubModuleName { get; set; }
    public bool? IsMandatory { get; set; }

}