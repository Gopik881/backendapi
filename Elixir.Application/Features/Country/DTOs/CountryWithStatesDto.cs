namespace Elixir.Application.Features.Country.DTOs;

public class StateSummaryDto
{
    public int StateId { get; set; }
    public string StateName { get; set; } = null!;
    public string StateShortName { get; set; } = null!;
    public string? Description { get; set; }
}

public class CountryWithStatesDto
{
    public int CountryId { get; set; }
    public string CountryName { get; set; }
    public IEnumerable<StateSummaryDto> States { get; set; }  
}
