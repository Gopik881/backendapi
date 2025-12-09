namespace Elixir.Application.Features.State.DTOs;
public class CreateUpdateStateDto
{
    public int? StateId { get; set; } = 0;
    public int CountryId { get; set; }
    public string StateName { get; set; } = null!;
    public string StateShortName { get; set; } = null!;
    public string? Description { get; set; }
}
