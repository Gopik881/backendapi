namespace Elixir.Application.Features.State.DTOs;
public class StateDto
{
    public int StateId { get; set; }
    public int CountryId { get; set; }
    public string CountryName { get; set; } = null;
    public string StateName { get; set; } = null!;
    public string StateShortName { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime? CreatedOn { get; set; }
}
