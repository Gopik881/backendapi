
namespace Elixir.Application.Features.Module.DTOs;

public class CompanyModuleDto
{
    public int CompanyId { get; set; }
    public string CompanyName { get; set; }
    public List<ModuleDto> Modules { get; set; } = null;
    public DateTime? CreatedOn { get; set; }
}
