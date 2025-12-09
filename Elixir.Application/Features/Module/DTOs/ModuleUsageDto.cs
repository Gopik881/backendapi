namespace Elixir.Application.Features.Module.DTOs;

public class ModuleUsageDto
{
    public int ModuleId { get; set; }
    public string ModuleName { get; set; } = string.Empty;

    public bool? Status { get; set; } = false;
    public int CompanyCount { get;set; } = 0;
    public int SubModuleCount { get; set; } = 0;
    public DateTime? CreatedOn { get; set; }
}
