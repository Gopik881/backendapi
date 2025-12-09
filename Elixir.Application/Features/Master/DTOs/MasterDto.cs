
namespace Elixir.Application.Features.Master.DTOs;

public class MasterDto
{
    public int MasterId { get; set; }
    public string MasterName { get; set; }    = string.Empty;
    public string MasterUrl { get; set; } = string.Empty;
    public DateTime? CreatedOn { get; set; }

}
