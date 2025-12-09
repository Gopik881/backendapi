namespace Elixir.Application.Features.Module.DTOs;

public class ModuleCreateDto
{
    public int? ModuleId { get; set; }
    public string ModuleName { get; set; }
    public string ModuleURL { get; set; }
    public string Description { get; set; }
    public bool? Status { get; set; }
}
